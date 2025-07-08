
namespace Huddle.Voice.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //builder.Services.AddSignalR().AddStackExchangeRedis("redis", options =>
        //{
        //    options.Configuration.AbortOnConnectFail = false;
        //    options.Configuration.ChannelPrefix = "huddle-voice-service";
        //});

        builder.AddRedisClient("redis");

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
