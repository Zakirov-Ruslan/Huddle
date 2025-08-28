namespace Huddle.ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        var app = builder.Build();

        app.UseRouting();
        app.UseHttpsRedirection();

        app.MapReverseProxy();

        app.UseCors(options =>
        {
            var spaUrl = Environment.GetEnvironmentVariable("SPA_URL")
                ?? throw new ArgumentNullException("SPA_URL environment variable not defined");

            options.WithOrigins(spaUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        });

        app.Run();
    }
}
