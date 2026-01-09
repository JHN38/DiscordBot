# DiscordBot.Tests.Functional

Functional tests using EF Core In-Memory database.

## Running Tests

```bash
dotnet test tests/DiscordBot.Tests.Functional
```

## Test Categories

| Category | Tests | Description |
|----------|-------|-------------|
| DiscordEntityManager | 4 | Message persistence, dependency handling, DM support |

## Comparison with Integration Tests

| Aspect | Functional (In-Memory) | Integration (SQL Server) |
|--------|------------------------|--------------------------|
| Speed | Fast (~1s) | Slower (~15s) |
| FK Constraints | Not enforced | Enforced |
| Cascade Rules | Not tested | Tested |
| Docker Required | No | Yes |

> **Note**: For accurate database behavior testing, prefer `DiscordBot.Tests.Integration` which uses real SQL Server via Testcontainers.

## Dependencies

- **xUnit** - Test framework
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database provider
