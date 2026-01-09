using Xunit;
using DiscordBot.Service.RecordKeeping.Persistence.Data;
using DiscordBot.Service.RecordKeeping.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace DiscordBot.Tests.Integration.Fixtures;

/// <summary>
/// Base class for integration tests using SQL Server.
/// Provides common setup and utilities for database testing.
/// </summary>
[Collection("SqlServer")]
public abstract class IntegrationTestBase : IAsyncLifetime
{
    private readonly SqlServerFixture _fixture;
    protected IDbContextFactory<AppDbContext> DbContextFactory { get; private set; } = null!;
    protected DiscordEntityManager EntityManager { get; private set; } = null!;

    protected IntegrationTestBase(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    public virtual async Task InitializeAsync()
    {
        // Reset database for each test class to ensure isolation
        await _fixture.ResetDatabaseAsync();
        
        DbContextFactory = _fixture.CreateDbContextFactory();
        EntityManager = new DiscordEntityManager(DbContextFactory, NullLogger<DiscordEntityManager>.Instance);
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Counts entities of a specific type in the database.
    /// </summary>
    protected async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        await using var db = await DbContextFactory.CreateDbContextAsync();
        return await db.Set<TEntity>().CountAsync();
    }

    /// <summary>
    /// Gets a fresh DbContext for querying (separate from EntityManager's contexts).
    /// </summary>
    protected async Task<AppDbContext> GetQueryContextAsync()
    {
        return await DbContextFactory.CreateDbContextAsync();
    }
}
