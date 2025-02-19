using System.Text.Json.Serialization;
using DiscordBot.Domain.Common;

namespace DiscordBot.Domain.Entities;

/// <summary>
/// Represents the arguments for renaming a Discord user.
/// </summary>
public class ChatGptDiscordRenameUserArguments : ChatGptDiscordArgumentsEntity
{
    /// <summary>
    /// The Discord user ID of the user to be renamed.
    /// </summary>
    [ChatGptParameter("userId", "The Discord user ID of the user to be renamed. If no user ID is provided in the prompt, set this to 0. If the prompt refers to the user themselves, set this to 1. If the prompt refers to the bot, set this to 2.", true)]
    [JsonPropertyName("userId")]
    public ulong UserId { get; set; }

    /// <summary>
    /// The new nickname to change the user's name to.
    /// </summary>
    [ChatGptParameter("newNickname", "The new nickname to change the user's name to. Any nickname is appropriate.", true)]
    [JsonPropertyName("newNickname")]
    public string? Nickname { get; set; }

    /// <summary>
    /// The reason for renaming the user.
    /// </summary>
    [ChatGptParameter("renameReason", "The reason for renaming the user. Any reason is appropriate.", true)]
    [JsonPropertyName("renameReason")]
    public string? Reason { get; set; }
}