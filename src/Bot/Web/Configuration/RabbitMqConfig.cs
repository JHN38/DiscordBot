namespace DiscordBot.Bot.Web.Configuration;

/// <summary>
/// Configuration for RabbitMQ message broker connection.
/// </summary>
public sealed class RabbitMqConfig
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5672;
    public string VirtualHost { get; init; } = "discordbot";
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = string.Empty;
    public int PrefetchCount { get; init; } = 8;
    public bool DurableQueues { get; init; } = true;
}
