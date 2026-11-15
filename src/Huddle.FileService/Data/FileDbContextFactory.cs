using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Huddle.FileService.Data;

public class FileDbContextFactory : IDesignTimeDbContextFactory<FileDbContext>
{
    public FileDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FileDbContext>()
            .UseNpgsql("Host=localhost;Database=filedb;Username=postgres")
            .Options;

        return new FileDbContext(options);
    }
}
