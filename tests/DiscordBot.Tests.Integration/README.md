# DiscordBot.Tests.Integration

Integration tests using real SQL Server via Testcontainers.

## Prerequisites

- **Docker** - Required for Testcontainers to spin up SQL Server

## Running Tests

```bash
# Run all integration tests
dotnet test tests/DiscordBot.Tests.Integration

# Run with verbose output
dotnet test tests/DiscordBot.Tests.Integration --verbosity normal
```

## Test Categories

| Category | Tests | Status |
|----------|-------|--------|
| DiscordEntityManager | 8 | ✅ Passing |
| Concurrency | 4 | ❌ Failing (exposes real bugs) |

## Concurrency Bug Discovery

The concurrency tests **intentionally fail** to expose race conditions in `DiscordEntityManager`:

```
Cannot insert duplicate key row in object 'dbo.Users' 
with unique index 'IX_Users_DiscordId'
```

This occurs when parallel Discord events try to create the same entity. Requires implementing retry/upsert logic in `DiscordEntityManager`.

## Infrastructure

### SqlServerFixture

Provides a real SQL Server 2022 instance:

```csharp
[Collection("SqlServer")]
public class MyTests : IntegrationTestBase
{
    public MyTests(SqlServerFixture fixture) : base(fixture) { }
    
    [Fact]
    public async Task My_test()
    {
        // EntityManager and DbContextFactory are available
        await EntityManager.GetOrAddMessageAsync(messageDto);
    }
}
```

### IntegrationTestBase

Base class providing:
- `EntityManager` - Pre-configured `DiscordEntityManager`
- `DbContextFactory` - For direct database queries
- `GetQueryContextAsync()` - Fresh DbContext for assertions
- Automatic database reset between test classes

## Dependencies

- **xUnit** - Test framework
- **FluentAssertions** - Assertion library
- **Testcontainers.MsSql** - SQL Server containers
