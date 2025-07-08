using Huddle.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Huddle.Identity.Data
{
    public class UsersSeeder(ILogger<UsersSeeder> logger, UserManager<ApplicationUser> userManager) : IDbSeeder<IdentityContext>
    {
        public async Task SeedAsync(IdentityContext context)
        {
            var alice = await userManager.FindByNameAsync("alice");

            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(alice, "Pass123$");
                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await context.SaveChangesAsync();
            }

            var bob = await userManager.FindByNameAsync("bob");

            if (bob == null)
            {
                alice = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(alice, "Pass123$");
                if (!result.Succeeded)
                    throw new Exception(result.Errors.First().Description);

                await context.SaveChangesAsync();
            }
        }
    }
}
