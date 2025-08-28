using Duende.IdentityServer.Models;
using Huddle.Identity.Configs;
using Huddle.Identity.Data;
using Huddle.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Huddle.Identity;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddRazorPages();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddDataProtection();

        builder.Services.AddDbContext<IdentityContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("identitydb")));

        builder.Services.AddMigration<IdentityContext, UsersSeeder>();

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

        builder.Services.AddIdentityServer(options =>
        {
            options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // TODO: Remove this line in production.
            options.KeyManagement.Enabled = false;


        })
        .AddInMemoryIdentityResources(Config.GetResources())
        .AddInMemoryApiScopes(Config.GetApiScopes())
        .AddInMemoryApiResources(Config.GetApis())
        .AddInMemoryClients(Config.GetClients())
        .AddAspNetIdentity<ApplicationUser>()
        // TODO: Not recommended for production - you need to store your key material somewhere secure
        .AddDeveloperSigningCredential();

        // JWT
        var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";").First();
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.Authority = url;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                };
            });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.MapDefaultEndpoints();
        app.UseStaticFiles();
        
        app.UseRouting();

        app.UseIdentityServer();

        app.UseAuthorization();
        app.UseAuthentication();
        
        app.MapRazorPages()
            .RequireAuthorization();

        app.Run();
    }
}
