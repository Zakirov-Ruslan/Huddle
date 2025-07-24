using Huddle.Channel.Infrastructure;
using Huddle.Channel.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Huddle.Channel.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.AddRabbitMqEventBus("eventbus")
            .AddEventBusSubscriptions();

        builder.EnrichNpgsqlDbContext<ChannelContext>();

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
            options.Audience = "huddle.channel.api"; 
            options.RequireHttpsMetadata = true; 

            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(2);
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors(options =>
        {
            var spaUrl = Environment.GetEnvironmentVariable("SPA_URL")
                ?? throw new ArgumentNullException("SPA_URL environment variable not defined");

            options.WithOrigins(spaUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.UseAuthorization();

        app.MapControllers().RequireAuthorization();

        app.Run();
    }
}
