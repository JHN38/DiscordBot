namespace DiscordBot.Service.RecordKeeping.Messaging;

/// <summary>
/// RabbitMQ topology constants for RecordKeeping service.
/// </summary>
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
