using Huddle.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Huddle.Identity.Data
{
    /// <remarks>
    /// Add migrations using the following command inside the 'Huddle.Identity.WebApi' project directory:
    ///
    /// dotnet ef migrations add [migration-name]
    /// </remarks>
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
