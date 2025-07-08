using Huddle.Channel.Infrastructure;
using Huddle.Channel.Infrastructure.Extensions;

namespace Huddle.Channel.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.AddRabbitMqEventBus("eventbus")
            .AddEventBusSubscriptions();

        builder.EnrichNpgsqlDbContext<ChannelContext>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
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

        app.MapControllers();

        app.Run();
    }
}
