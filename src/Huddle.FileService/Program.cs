using Huddle.FileService.Data;
using Huddle.FileService.Endpoints;
using Huddle.FileService.Options;
using Huddle.FileService.Services;
using Huddle.FileService.Storage;
using Huddle.FileService.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;

namespace Huddle.FileService;

public class Program
{
    private const long MaxRequestBodySizeBytes = 64L * 1024 * 1024;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = MaxRequestBodySizeBytes;
        });

        var fileDbConnectionString = builder.Configuration.GetConnectionString("filedb") ?? string.Empty;
        builder.Services.AddDbContext<FileDbContext>(options => options.UseNpgsql(fileDbConnectionString));
        builder.EnrichNpgsqlDbContext<FileDbContext>();

        var minioConnection = MinioConnection.Parse(builder.Configuration.GetConnectionString(StorageOptions.MinioConnectionStringName));

        builder.Services.AddOptions<StorageOptions>()
            .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
            .PostConfigure(options =>
            {
                options.Endpoint = FirstNonEmpty(options.Endpoint, minioConnection.Endpoint);
                options.AccessKey = FirstNonEmpty(options.AccessKey, minioConnection.AccessKey);
                options.SecretKey = FirstNonEmpty(options.SecretKey, minioConnection.SecretKey);
            })
            .Validate(options => !string.IsNullOrWhiteSpace(options.Endpoint), "MinIO endpoint is not configured")
            .Validate(options => !string.IsNullOrWhiteSpace(options.AccessKey), "MinIO access key is not configured")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SecretKey), "MinIO secret key is not configured")
            .ValidateOnStart();

        builder.Services.AddSingleton<IMinioClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<StorageOptions>>().Value;
            var (endpoint, useSsl) = ParseMinioEndpoint(options.Endpoint!, options.UseSsl);

            return new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(useSsl)
                .Build();
        });

        builder.Services.AddSingleton<IFileStorage, MinioFileStorage>();
        builder.Services.AddSingleton<FileUploadValidator>();

        builder.Services.AddHostedService<DatabaseMigrationHostedService>();
        builder.Services.AddHostedService<MinioBucketInitializer>();
        builder.Services.AddHostedService<PendingFileCleanupService>();

        var identityUrl = Environment.GetEnvironmentVariable("IDENTITY_URL")
            ?? throw new ArgumentNullException("IDENTITY_URL environment variable not defined");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.Audience = "huddle.files.api";
            options.RequireHttpsMetadata = true;

            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidIssuer = identityUrl;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(2);
        });

        builder.Services.AddAuthorization();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // The service is only reachable through the gateway over plain HTTP inside the cluster,
        // so there is no HTTPS redirect here.
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapFileEndpoints();

        app.Run();
    }

    private static string? FirstNonEmpty(string? value, string? fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;

    private static (string Endpoint, bool UseSsl) ParseMinioEndpoint(string endpoint, bool useSsl)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return (endpoint, useSsl);

        var host = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";

        return (host, uri.Scheme == Uri.UriSchemeHttps);
    }
}
