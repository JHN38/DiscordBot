# RabbitMQ Integration Plan for DiscordBot Microservices

## Executive Summary

This plan outlines the integration of RabbitMQ as a message broker between the DiscordBot microservices. The current architecture uses Wolverine with TCP transport; this plan upgrades to RabbitMQ to enable **horizontal scaling** of services, particularly RecordKeeping which may need multiple instances to handle high message volumes.

---

## Scaling Goal

The primary driver for RabbitMQ is **horizontal scaling via competing consumers**:

```
                    RabbitMQ
                        |
            [recordkeeping.commands queue]
                        |
        +-------+-------+-------+
        |       |       |       |
        v       v       v       v
   Instance  Instance  Instance  Instance
      1         2         3         4
        \       |       |       /
         \      |       |      /
          +-----+-------+-----+
                    |
              SQL Server
```

When demand grows, deploy additional RecordKeeping instances. RabbitMQ automatically distributes messages across all consumers.

---

## Current Architecture Analysis

### Existing Services

| Service | Purpose | Current Transport |
|---------|---------|-------------------|
| **Bot** (`src/Bot/Web`) | Discord gateway, event orchestration | TCP Publisher (ports 5001-5003) |
| **RecordKeeping** (`src/Services/RecordKeeping`) | Message persistence to SQL Server | TCP Listener (port 5001) |
| **Weather** (`src/Services/Weather`) | OpenWeatherMap API integration | TCP Listener (port 5002) |
| **WebSearch** (`src/Services/WebSearch`) | Google/SerpApi integration | TCP Listener (port 5003) |

### Current Communication Patterns

- **Fire-and-forget**: RecordKeeping commands (`SaveMessageCommand`, `SaveGuildCommand`, etc.)
- **Request/Response**: Weather queries (`GetWeatherQuery` -> `WeatherResponse`)
- **Request/Response**: WebSearch queries (`SearchWebQuery` -> `WebSearchResponse`)

---

## Why RabbitMQ?

### Problems Solved

| Problem | TCP Transport | RabbitMQ |
|---------|---------------|----------|
| Horizontal scaling | Not possible | Competing consumers |
| Message persistence during service restart | Messages lost | Durable queues |
| Service unavailable handling | Connection failure | Queue buffering |
| Retry with backoff | Manual implementation | Built-in |
| Dead letter handling | None | DLQ support |
| Observability | Logs only | Management UI + metrics |

---

## Proposed Architecture

### Message Queue Topology

```
                        RabbitMQ
                            |
            +---------------+---------------+
            |               |               |
      [discordbot Exchange - Topic]
            |               |               |
            v               v               v
    recordkeeping.#    weather.#     websearch.#
            |               |               |
            v               v               v
[recordkeeping.commands] [weather.requests] [websearch.requests]
            |               |               |
     (competing)       (single)        (single)
      consumers        consumer        consumer
            |               |               |
            v               v               v
   RecordKeeping x N    Weather         WebSearch

Dead Letter:
[discordbot.dlx] -> [*.dlq] per service
```

### Design Decisions

| Decision | Choice | Rationale |
|----------|--------|-----------|
| Exchange type | Single Topic | Simplicity; route by service prefix |
| Queues | 3 command queues + 3 DLQs | One per service with failure isolation |
| VirtualHost | `discordbot` | Security isolation |
| Durability | Durable queues, persistent messages | Survive broker restarts |
| Acknowledgments | Manual ack | At-least-once delivery |
| RecordKeeping consumers | Multiple (competing) | Horizontal scaling |
| Concurrency strategy | Atomic updates (`ExecuteUpdateAsync`) | Simple, no RowVersion needed, last-write-wins |

---

## Prerequisites: Code Changes for Competing Consumers

Before enabling multiple RecordKeeping instances, the following code changes are **required** to ensure safe concurrent processing.

### 1. Convert Handlers to Atomic Operations

The modern approach uses `ExecuteUpdateAsync` for atomic database updates. This eliminates race conditions without requiring optimistic concurrency tokens (RowVersion).

**Why `ExecuteUpdateAsync`?**
- Atomic: Single SQL statement, no read-modify-write race
- Simple: No RowVersion columns, no concurrency exceptions to handle
- Last-write-wins: Acceptable for Discord data where updates are milliseconds apart

**Current pattern** (unsafe with competing consumers):

```csharp
// Read-modify-write has a race condition window
var message = await db.Messages.FirstOrDefaultAsync(m => m.DiscordId == command.MessageId);
message.Content = command.Content;
await db.SaveChangesAsync();
```

**Required pattern** (atomic update):

```csharp
// Atomic - safe with competing consumers, last-write-wins
await db.Messages
    .Where(m => m.DiscordId == command.MessageId)
    .ExecuteUpdateAsync(s => s
        .SetProperty(m => m.Content, command.Content)
        .SetProperty(m => m.IsEdited, true)
        .SetProperty(m => m.EditedTimestamp, command.EditedTimestamp),
        cancellationToken);
```

**Handlers requiring update**:

| Handler | File | Current Pattern | Required Change |
|---------|------|-----------------|-----------------|
| `UpdateMessageHandler` | `MessageHandlers.cs` | Read-modify-write | `ExecuteUpdateAsync` |
| `DeleteMessageHandler` | `MessageHandlers.cs` | Read-modify-write | `ExecuteUpdateAsync` |
| `UpdateBanHandler` | `BanHandlers.cs` | Read-modify-write | `ExecuteUpdateAsync` |
| `UpdateVoiceSessionHandler` | `VoiceSessionHandlers.cs` | Read-modify-write | `ExecuteUpdateAsync` |
| `RemoveReactionHandler` | `ReactionHandlers.cs` | Read-modify-write | `ExecuteDeleteAsync` |

> **Note**: `BulkDeleteMessagesHandler` already uses `ExecuteUpdateAsync` - use it as a reference.

### 2. Configure Database Connection Pool and Resilience

With multiple instances, connection pool exhaustion and transient failures become more likely.

**File**: `src/Services/RecordKeeping/Persistence/DependencyInjection.cs`

```csharp
optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null);
    sqlOptions.CommandTimeout(30);
});
```

**Connection string** (appsettings.json):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Max Pool Size=50;Connection Timeout=30;..."
  }
}
```

### 3. Standardize Dependency Handling

Some handlers throw on missing dependencies, others create them. Standardize to **self-healing** (recommended for competing consumers):

```csharp
// Ensure dependencies exist before processing
var user = await db.Users.GetOrCreateByDiscordIdAsync(...);
var channel = await db.Channels.GetOrCreateByDiscordIdAsync(...);
```

This pattern already exists in most handlers via `IDiscordEntityManager`.

---

## Implementation Plan

### Phase 1: Infrastructure Setup

#### 1.1 Update docker-compose.yml

**File**: `/home/josse/repos/DiscordBot/docker-compose.yml`

```yaml
services:
  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: discordbot-rabbitmq
    hostname: rabbitmq
    ports:
      - "5672:5672"    # AMQP
      - "15672:15672"  # Management UI
    environment:
      - RABBITMQ_DEFAULT_USER=${RABBITMQ_USER:-discordbot}
      - RABBITMQ_DEFAULT_PASS=${RABBITMQ_PASS}
      - RABBITMQ_DEFAULT_VHOST=discordbot
    volumes:
      - rabbitmq-data:/var/lib/rabbitmq
    networks:
      - discordbot-network
    healthcheck:
      test: ["CMD", "rabbitmq-diagnostics", "-q", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5
      start_period: 30s

  # Update existing services to depend on RabbitMQ
  bot:
    depends_on:
      rabbitmq:
        condition: service_healthy
    environment:
      - RabbitMQ__Host=rabbitmq
      - RabbitMQ__VirtualHost=discordbot
      - RabbitMQ__Username=${RABBITMQ_USER:-discordbot}
      - RabbitMQ__Password=${RABBITMQ_PASS}

  recordkeeping:
    depends_on:
      rabbitmq:
        condition: service_healthy
    environment:
      - RabbitMQ__Host=rabbitmq
      - RabbitMQ__VirtualHost=discordbot
      - RabbitMQ__Username=${RABBITMQ_USER:-discordbot}
      - RabbitMQ__Password=${RABBITMQ_PASS}
    deploy:
      replicas: 1  # Increase when scaling needed

  # Similar for weather and websearch services

volumes:
  rabbitmq-data:
```

#### 1.2 Add NuGet Package

**File**: `/home/josse/repos/DiscordBot/Directory.Build.props`

```xml
<PackageReference Include="WolverineFx.RabbitMQ" Version="5.9.2" />
```

#### 1.3 Environment Variables

**File**: `.env`

```bash
RABBITMQ_USER=discordbot
RABBITMQ_PASS=<secure-password>
```

---

### Phase 2: Messaging Infrastructure

#### 2.1 Configuration Class

> **Clean Architecture Note**: Configuration classes are infrastructure concerns and should NOT be placed in the Contracts project. Each service should have its own copy or use a shared Infrastructure project.

**New File**: `/home/josse/repos/DiscordBot/src/Services/RecordKeeping/Service/Configuration/RabbitMqConfig.cs`

```csharp
namespace DiscordBot.Service.RecordKeeping.Configuration;

public sealed class RabbitMqConfig
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "discordbot";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = string.Empty;

    public int PrefetchCount { get; set; } = 8;  // Tuned for DB-heavy workloads
    public bool DurableQueues { get; set; } = true;
}
```

Create the same file in each service that needs RabbitMQ (Bot, Weather, WebSearch).

#### 2.2 Queue Topology Constants

> **Clean Architecture Note**: Queue names are infrastructure topology, not domain contracts. Keep them in infrastructure.

**New File**: `/home/josse/repos/DiscordBot/src/Services/RecordKeeping/Service/Messaging/RabbitMqTopology.cs`

```csharp
namespace DiscordBot.Service.RecordKeeping.Messaging;

internal static class RabbitMqTopology
{
    // Exchange
    public const string MainExchange = "discordbot";
    public const string DeadLetterExchange = "discordbot.dlx";

    // This service's queue
    public const string CommandsQueue = "recordkeeping.commands";
    public const string DeadLetterQueue = "recordkeeping.dlq";

    // Routing
    public const string BindingKey = "recordkeeping.#";
}
```

---

### Phase 3: Service Configuration

#### 3.1 Bot Service (Publisher)

**File**: `/home/josse/repos/DiscordBot/src/Bot/Web/Program.cs`

Replace TCP transport configuration:

```csharp
using Wolverine.RabbitMQ;

var rabbitConfig = builder.Configuration
    .GetSection("RabbitMQ")
    .Get<RabbitMqConfig>() ?? new RabbitMqConfig();

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = rabbitConfig.Host;
        rabbit.Port = rabbitConfig.Port;
        rabbit.VirtualHost = rabbitConfig.VirtualHost;
        rabbit.UserName = rabbitConfig.Username;
        rabbit.Password = rabbitConfig.Password;
    })
    .AutoProvision()
    .UseConventionalRouting();

    // Publish RecordKeeping commands (fire-and-forget)
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.RecordKeeping");
        rule.ToRabbitExchange("discordbot", e => e.ExchangeType = ExchangeType.Topic);
    });

    // Publish Weather queries (request/response)
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.Weather");
        rule.ToRabbitExchange("discordbot", e => e.ExchangeType = ExchangeType.Topic);
    });

    // Publish WebSearch queries (request/response)
    opts.Publish(rule =>
    {
        rule.MessagesFromNamespace("DiscordBot.Contracts.WebSearch");
        rule.ToRabbitExchange("discordbot", e => e.ExchangeType = ExchangeType.Topic);
    });

    // RPC reply handling - Wolverine automatically sets ReplyUri header
    // Responses are routed back via the default reply queue
    opts.ListenToRabbitQueue("bot.replies")
        .ProcessInline();

    opts.Discovery.IncludeAssembly(AssemblyReference.Assembly);
}, ExtensionDiscovery.ManualOnly);
```

#### 3.2 RecordKeeping Service (Consumer - Scalable)

**File**: `/home/josse/repos/DiscordBot/src/Services/RecordKeeping/Service/Program.cs`

```csharp
using Wolverine.RabbitMQ;
using RabbitMQ.Client.Exceptions;

var rabbitConfig = builder.Configuration
    .GetSection("RabbitMQ")
    .Get<RabbitMqConfig>() ?? new RabbitMqConfig();

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
    {
        rabbit.HostName = rabbitConfig.Host;
        rabbit.Port = rabbitConfig.Port;
        rabbit.VirtualHost = rabbitConfig.VirtualHost;
        rabbit.UserName = rabbitConfig.Username;
        rabbit.Password = rabbitConfig.Password;
    })
    .AutoProvision();

    // Listen to commands queue - supports competing consumers
    opts.ListenToRabbitQueue(RabbitMqTopology.CommandsQueue, queue =>
    {
        queue.PreFetchCount(rabbitConfig.PrefetchCount);
        queue.UseDurableQueues();
        queue.DeadLetterQueueing(new DeadLetterQueue(RabbitMqTopology.DeadLetterQueue));
    })
    .BindToExchange(RabbitMqTopology.MainExchange, binding =>
    {
        binding.ExchangeType = ExchangeType.Topic;
        binding.BindingKey = RabbitMqTopology.BindingKey;
    });

    // Retry policy for database operations
    opts.Policies.OnException<DbUpdateException>()
        .RetryWithCooldown(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15))
        .Then.MoveToErrorQueue();

    // Retry policy for RabbitMQ connection issues during startup
    opts.Policies.OnException<BrokerUnreachableException>()
        .RetryWithCooldown(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30))
        .Then.MoveToErrorQueue();

}, ExtensionDiscovery.ManualOnly);
```

#### 3.3 Weather Service (Consumer with RPC Response)

**File**: `/home/josse/repos/DiscordBot/src/Services/Weather/Service/Program.cs`

```csharp
opts.UseRabbitMq(rabbit =>
{
    rabbit.HostName = rabbitConfig.Host;
    rabbit.Port = rabbitConfig.Port;
    rabbit.VirtualHost = rabbitConfig.VirtualHost;
    rabbit.UserName = rabbitConfig.Username;
    rabbit.Password = rabbitConfig.Password;
})
.AutoProvision();

// Listen to weather queue
opts.ListenToRabbitQueue("weather.requests", queue =>
{
    queue.PreFetchCount(rabbitConfig.PrefetchCount);
    queue.UseDurableQueues();
    queue.DeadLetterQueueing(new DeadLetterQueue("weather.dlq"));
})
.BindToExchange("discordbot", binding =>
{
    binding.ExchangeType = ExchangeType.Topic;
    binding.BindingKey = "weather.#";
});

// Wolverine automatically routes responses back to the caller's ReplyUri
// No explicit reply configuration needed - handlers just return the response type

opts.Policies.OnException<HttpRequestException>()
    .RetryWithCooldown(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5))
    .Then.MoveToErrorQueue();
```

> **RPC Pattern Note**: Wolverine handles request/response over RabbitMQ automatically. When Bot sends `GetWeatherQuery`, Wolverine includes a `ReplyUri` header. The Weather service handler returns `WeatherResponse`, and Wolverine routes it back to the Bot's reply queue.

#### 3.4 WebSearch Service (Consumer)

Similar pattern to Weather Service with `websearch.#` binding key.

---

### Phase 4: Health Checks & Monitoring

#### 4.1 Add Health Check Package

Add to each service `.csproj`:

```xml
<PackageReference Include="AspNetCore.HealthChecks.Rabbitmq" Version="8.0.1" />
```

#### 4.2 Register Health Checks

In each service's `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddRabbitMQ(
        $"amqp://{rabbitConfig.Username}:{rabbitConfig.Password}@{rabbitConfig.Host}:{rabbitConfig.Port}/{rabbitConfig.VirtualHost}",
        name: "rabbitmq",
        tags: ["ready"]);
```

---

## Graceful Shutdown

When scaling down or redeploying, handle in-flight messages properly.

### Configuration

```csharp
builder.Host.UseWolverine(opts =>
{
    // Wait for in-flight messages before shutdown
    opts.Durability.Mode = DurabilityMode.Solo;

    // Drain timeout - how long to wait for in-flight messages
    opts.Durability.ScheduledJobPollingTime = TimeSpan.FromSeconds(5);
});

// In Program.cs - configure graceful shutdown
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});
```

### Behavior

- On shutdown signal (SIGTERM), Wolverine stops accepting new messages
- In-flight messages complete processing (up to shutdown timeout)
- Unacked messages return to the queue for other instances

---

## Scaling RecordKeeping

### When to Scale

Monitor these metrics in RabbitMQ Management UI:

| Metric | Threshold | Action |
|--------|-----------|--------|
| Queue depth (recordkeeping.commands) | > 1000 sustained | Add instance |
| Consumer utilization | > 80% | Add instance |
| Message processing time | > 500ms average | Investigate bottleneck |

### How to Scale

**Docker Compose** (development):

```yaml
recordkeeping:
  deploy:
    replicas: 3
```

**Kubernetes** (production):

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: recordkeeping
spec:
  replicas: 3  # Scale as needed
```

### Scaling Limits

| Constraint | Limit | Mitigation |
|------------|-------|------------|
| SQL Server connections | ~100 per instance | Configure Max Pool Size |
| RabbitMQ connections | ~1000 | Use connection pooling |
| Database write throughput | Varies | Consider read replicas, sharding by GuildId |

---

## Testing Strategy

### Integration Tests with Testcontainers

**New File**: `/home/josse/repos/DiscordBot/tests/DiscordBot.Tests.Integration/Fixtures/RabbitMqFixture.cs`

```csharp
using Testcontainers.RabbitMq;

public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _container = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management-alpine")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    public string Host => _container.Hostname;
    public int Port => _container.GetMappedPublicPort(5672);

    public async Task InitializeAsync() => await _container.StartAsync();
    public async Task DisposeAsync() => await _container.DisposeAsync();
}
```

**Package Reference**:

```xml
<PackageReference Include="Testcontainers.RabbitMq" Version="4.3.0" />
```

### Competing Consumer Test

```csharp
[Fact]
public async Task Multiple_Instances_Should_Process_Messages_In_Parallel()
{
    // Arrange - track which instance processed each message
    var processedBy = new ConcurrentDictionary<ulong, string>();

    // Start two consumer hosts with different instance IDs
    var consumer1 = await CreateConsumerHost("instance-1", processedBy);
    var consumer2 = await CreateConsumerHost("instance-2", processedBy);

    // Act - send multiple messages
    var messageIds = Enumerable.Range(1, 100).Select(i => (ulong)i).ToList();
    foreach (var id in messageIds)
    {
        await bus.SendAsync(new SaveMessageCommand(id, ...));
    }

    // Wait for processing
    await Task.Delay(5000);

    // Assert - both instances should have processed messages
    var instance1Count = processedBy.Values.Count(v => v == "instance-1");
    var instance2Count = processedBy.Values.Count(v => v == "instance-2");

    Assert.True(instance1Count > 0, "Instance 1 should process some messages");
    Assert.True(instance2Count > 0, "Instance 2 should process some messages");
    Assert.Equal(100, instance1Count + instance2Count);
}
```

---

## Migration Strategy

### Recommended: Gradual Migration

1. **Phase 1**: Deploy RabbitMQ alongside TCP (feature flag)
2. **Phase 2**: Migrate RecordKeeping to RabbitMQ (single instance first)
3. **Phase 3**: Apply prerequisite code changes (atomic updates)
4. **Phase 4**: Scale RecordKeeping to 2+ instances, validate
5. **Phase 5**: Migrate Weather and WebSearch
6. **Phase 6**: Remove TCP transport

---

## Files Summary

### New Files

| Path | Purpose |
|------|---------|
| `src/Services/*/Service/Configuration/RabbitMqConfig.cs` | Configuration per service |
| `src/Services/*/Service/Messaging/RabbitMqTopology.cs` | Queue constants per service |
| `tests/.../Fixtures/RabbitMqFixture.cs` | Test infrastructure |

### Modified Files

| Path | Changes |
|------|---------|
| `docker-compose.yml` | Add RabbitMQ service, update dependencies |
| `Directory.Build.props` | Add WolverineFx.RabbitMQ package |
| `src/Bot/Web/Program.cs` | Replace TCP with RabbitMQ publisher |
| `src/Services/RecordKeeping/Service/Program.cs` | Replace TCP with RabbitMQ consumer |
| `src/Services/RecordKeeping/Service/Handlers/MessageHandlers.cs` | Convert to `ExecuteUpdateAsync` |
| `src/Services/RecordKeeping/Service/Handlers/BanHandlers.cs` | Convert to `ExecuteUpdateAsync` |
| `src/Services/RecordKeeping/Service/Handlers/VoiceSessionHandlers.cs` | Convert to `ExecuteUpdateAsync` |
| `src/Services/RecordKeeping/Service/Handlers/ReactionHandlers.cs` | Convert to `ExecuteDeleteAsync` |
| `src/Services/RecordKeeping/Persistence/DependencyInjection.cs` | Add `EnableRetryOnFailure` |
| `src/Services/Weather/Service/Program.cs` | Replace TCP with RabbitMQ consumer |
| `src/Services/WebSearch/Service/Program.cs` | Replace TCP with RabbitMQ consumer |
| `.env` | Add RABBITMQ_USER, RABBITMQ_PASS |

---

## Critical Considerations

### Concurrency Strategy: Atomic Updates

We use `ExecuteUpdateAsync` (last-write-wins) instead of optimistic concurrency with RowVersion:

| Approach | Pros | Cons |
|----------|------|------|
| **ExecuteUpdateAsync** (chosen) | Simple, no extra columns, no exceptions | Last-write-wins, no conflict detection |
| RowVersion | Detects conflicts | Complex, requires handling `DbUpdateConcurrencyException` |

For Discord data where updates happen milliseconds apart, last-write-wins is acceptable. Users won't notice if edit A overwrites edit B when they're near-simultaneous.

### Idempotency (Already Implemented)

Your existing `GetOrCreateByDiscordIdAsync` pattern with unique constraint retry is correct:

```csharp
catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
{
    // Another consumer created it first - retrieve and continue
}
```

Discord IDs serve as natural idempotency keys.

### Security

- Use VirtualHost isolation (`discordbot`)
- TLS in production (configure via connection string)
- Service-specific users with minimal permissions (future enhancement)

---

## Monitoring

### RabbitMQ Management UI

Access at `http://localhost:15672` (credentials from `.env`)

### Key Metrics to Watch

| Metric | Location | Alert Threshold |
|--------|----------|-----------------|
| Queue depth | Queues tab | > 1000 messages |
| Consumer count | Queue details | < expected instances |
| DLQ depth | Queues tab | > 0 (investigate failures) |
| Unacked messages | Queue details | Growing = slow consumers |

---

## Rollback Plan

If issues arise:

1. Revert `Program.cs` changes to TCP transport
2. Scale RecordKeeping back to 1 instance
3. Remove RabbitMQ from `docker-compose.yml`
4. Redeploy

Wolverine's transport abstraction makes rollback straightforward.

---

## Appendix: Review Summary

### Code Critic Findings (Addressed)

| Concern | Resolution |
|---------|------------|
| Over-engineered routing keys | Simplified to service-level routing (`recordkeeping.#`) |
| Missing idempotency strategy | Existing pattern is correct; documented |
| Wrong entity file path | Removed RowVersion requirement entirely |
| Missing handlers in update list | Added `DeleteMessageHandler`, `RemoveReactionHandler` |
| Muddled concurrency strategy | Chose `ExecuteUpdateAsync` only, removed RowVersion |
| RPC reply routing unclear | Documented Wolverine automatic ReplyUri handling |
| Missing graceful shutdown | Added shutdown configuration section |
| RabbitMQ connection failures | Added `BrokerUnreachableException` retry policy |

### Clean Architecture Findings (Addressed)

| Concern | Resolution |
|---------|------------|
| RabbitMqConfig in Contracts | Moved to each service's Configuration folder |
| Queue constants in Contracts | Moved to each service's Messaging folder |
| Update handlers unsafe for concurrency | Documented all handlers requiring `ExecuteUpdateAsync` |
| Connection pool exhaustion risk | Documented `Max Pool Size` and `EnableRetryOnFailure` |

---

*Generated from consolidated plans reviewed by code critic and clean architecture agents*
