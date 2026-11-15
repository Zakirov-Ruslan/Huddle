using Microsoft.EntityFrameworkCore;

namespace Huddle.FileService.Data;

/// <summary>
/// Add migrations using the following command inside the solution directory:
/// dotnet ef migrations add [migration-name] --project src\Huddle.FileService --startup-project src\Huddle.FileService
/// </summary>
public class FileDbContext : DbContext
{
    public DbSet<StoredFile> Files => Set<StoredFile>();

    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Files");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileDbContext).Assembly);
    }
}
