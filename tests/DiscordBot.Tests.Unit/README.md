# DiscordBot.Tests.Unit

Fast, isolated unit tests with mocked dependencies.

## Running Tests

```bash
dotnet test tests/DiscordBot.Tests.Unit
```

## Test Categories

| Category | Tests | Description |
|----------|-------|-------------|
| Weather | 7 | `GetWeatherHandler` type parsing, null handling, response mapping |
| Mapping | 7 | `CommandMapper` edge cases for null guild handling |

## Dependencies

- **xUnit** - Test framework
- **NSubstitute** - Mocking framework
- **FluentAssertions** - Assertion library
- **RichardSzalay.MockHttp** - HTTP mocking

## Test Builders

Fluent builders for creating test data:

```csharp
var command = new SaveMessageCommandBuilder()
    .WithMessageId(123)
    .InGuild(456, "Test Guild")
    .AsBot()
    .Build();
```

Available builders:
- `SaveMessageCommandBuilder` - For `SaveMessageCommand` instances
- `DiscordMessageDtoBuilder` - For `DiscordMessageDto` instances
