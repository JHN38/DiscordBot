using Testcontainers.RabbitMq;
using Xunit;

namespace DiscordBot.Tests.Integration.Fixtures;

/// <summary>
/// Provides a real RabbitMQ instance via Testcontainers for integration testing.
/// This enables testing message broker functionality with actual RabbitMQ behavior.
/// </summary>
public sealed class RabbitMqFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _container = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management-alpine")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    /// <summary>
    /// Hostname of the running RabbitMQ container.
    /// </summary>
    public string Host => _container.Hostname;

    /// <summary>
    /// Mapped port for AMQP connections.
    /// </summary>
    public int Port => _container.GetMappedPublicPort(5672);

    /// <summary>
    /// Connection string for RabbitMQ.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync() => await _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();
}

/// <summary>
/// Collection definition for tests sharing the RabbitMQ container.
/// Tests in this collection share a single container instance for performance.
/// </summary>
[CollectionDefinition("RabbitMQ")]
public class RabbitMqCollection : ICollectionFixture<RabbitMqFixture> { }
