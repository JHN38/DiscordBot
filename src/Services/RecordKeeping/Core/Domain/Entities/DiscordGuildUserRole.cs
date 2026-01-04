namespace DiscordBot.Service.RecordKeeping.Core.Domain.Entities;

/// <summary>
/// Join table linking users to roles within guilds.
/// Uses composite key (GuildId, UserId, RoleId) instead of surrogate Id.
/// </summary>
public class DiscordGuildUserRole
{
    public int GuildId { get; set; }
    public virtual DiscordGuild Guild { get; set; } = default!;

    public int UserId { get; set; }
    public virtual DiscordUser User { get; set; } = default!;

    public int RoleId { get; set; }
    public virtual DiscordRole Role { get; set; } = default!;

    public DateTimeOffset AssignedAt { get; set; }

    public int? AssignedById { get; set; }
    public virtual DiscordUser? AssignedBy { get; set; }
}
