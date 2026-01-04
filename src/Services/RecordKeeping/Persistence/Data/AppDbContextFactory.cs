using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data;

/// <summary>
/// Design-time factory for creating AppDbContext instances.
/// Used by EF Core tooling for migrations.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Use SQL Server with a placeholder connection string for design-time operations
        // The actual connection string will be provided at runtime through configuration
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=DiscordBot_Design;Trusted_Connection=True;MultipleActiveResultSets=true",
            options => options.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name));

        return new AppDbContext(optionsBuilder.Options);
    }
}
