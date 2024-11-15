using System.Text.Json.Serialization;
using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents the arguments required to timeout a user on Discord using GPT.
/// </summary>
[ChatGptProperties]
public class ChatGptDiscordTimeoutUserArguments : ChatGptDiscordArgumentsEntity
{
    /// <summary>
    /// Gets or sets the Discord user ID of the user to be timed out.
    /// If no user ID is provided in the prompt, set this to 0.
    /// If the timeout concerns the author of the request, set this to 1.
    /// </summary>
    [ChatGptParameter("userId", "The Discord user ID of the user to be timed out. If no user ID is provided in the prompt, set this to 0. If the timeout concerns the author of the request, set this to 1.", true)]
    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    /// <summary>
    /// Gets or sets the duration of the timeout in seconds.
    /// Defaults to a reasonably low amount if no duration can be inferred.
    /// </summary>
    [ChatGptParameter("timeoutDuration", "The duration of the timeout in seconds. Default to a reasonably low amount if no duration can be inferred.", true)]
    [JsonPropertyName("timeoutDuration")]
    public int Duration { get; set; } = 60;

    /// <summary>
    /// Gets or sets the reason for timing out the user.
    /// Any reason is appropriate.
    /// </summary>
    [ChatGptParameter("timeoutReason", "The reason for timing out the user. Any reason is appropriate.", true)]
    [JsonPropertyName("timeoutReason")]
    public string? Reason { get; set; }
}