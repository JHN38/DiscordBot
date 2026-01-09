using Xunit;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using DiscordBot.Service.RecordKeeping.Persistence.Data;

namespace DiscordBot.Tests.Integration.Fixtures;

/// <summary>
/// Provides a real SQL Server instance via Testcontainers for integration testing.
/// This ensures tests run against actual SQL Server behavior (foreign keys, cascade rules, etc.)
/// rather than the limited In-Memory provider.
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    private const string TestDatabaseName = "DiscordBotTests";
    
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("Test@Password123!")
        .Build();

    /// <summary>
    /// Connection string to the running SQL Server container with our test database.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            // Get base connection string and replace master with our test database
            var baseConnectionString = _container.GetConnectionString();
            return baseConnectionString.Replace("Database=master", $"Database={TestDatabaseName}");
        }
    }

    /// <summary>
    /// Creates a new DbContextFactory configured for the test database.
    /// Each test should use this to get isolated DbContext instances.
    /// </summary>
    public IDbContextFactory<AppDbContext> CreateDbContextFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        return new TestDbContextFactory(options);
    }

    /// <summary>
    /// Creates a fresh database with schema applied.
    /// Call this before each test or test class to ensure isolation.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateDbContextFactory().CreateDbContext();
        
        // Drop and recreate the test database (not master!)
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        // Create the initial database
        await using var context = CreateDbContextFactory().CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    private sealed class TestDbContextFactory(DbContextOptions<AppDbContext> options) 
        : IDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext() => new(options);
        public Task<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) 
            => Task.FromResult(new AppDbContext(options));
    }
}

/// <summary>
/// Collection definition for tests sharing the SQL Server container.
/// Tests in this collection share a single container instance for performance.
/// </summary>
[CollectionDefinition("SqlServer")]
public class SqlServerCollection : ICollectionFixture<SqlServerFixture> { }
