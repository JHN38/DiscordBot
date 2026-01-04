using System.Text.Json;
using DiscordBot.Service.RecordKeeping.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DiscordBot.Service.RecordKeeping.Persistence.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    // Existing DbSets
    public DbSet<DiscordMessage> Messages => Set<DiscordMessage>();
    public DbSet<DiscordUser> Users => Set<DiscordUser>();
    public DbSet<DiscordChannel> Channels => Set<DiscordChannel>();
    public DbSet<DiscordGuild> Guilds => Set<DiscordGuild>();

    // New DbSets
    public DbSet<DiscordRole> Roles => Set<DiscordRole>();
    public DbSet<DiscordInvite> Invites => Set<DiscordInvite>();
    public DbSet<DiscordReaction> Reactions => Set<DiscordReaction>();
    public DbSet<DiscordVoiceSession> VoiceSessions => Set<DiscordVoiceSession>();
    public DbSet<DiscordPresenceLog> PresenceLogs => Set<DiscordPresenceLog>();
    public DbSet<DiscordUserBan> UserBans => Set<DiscordUserBan>();
    public DbSet<DiscordScheduledEvent> ScheduledEvents => Set<DiscordScheduledEvent>();
    public DbSet<DiscordAttachment> Attachments => Set<DiscordAttachment>();
    public DbSet<DiscordEmbed> Embeds => Set<DiscordEmbed>();
    public DbSet<DiscordSticker> Stickers => Set<DiscordSticker>();
    public DbSet<DiscordAuditLog> AuditLogs => Set<DiscordAuditLog>();
    public DbSet<DiscordUserNickname> UserNicknames => Set<DiscordUserNickname>();
    public DbSet<DiscordGuildUserRole> GuildUserRoles => Set<DiscordGuildUserRole>();
    public DbSet<DiscordMessageSticker> MessageStickers => Set<DiscordMessageSticker>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DiscordGuild Entity Configurations
        modelBuilder.Entity<DiscordGuild>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            // JSON conversion for Features list with proper value comparer
            // Handle null/empty strings gracefully by returning empty list
            entity.Property(e => e.Features)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => string.IsNullOrWhiteSpace(v)
                          ? new List<string>()
                          : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                  .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                      (c1, c2) => c1 != null && c2 != null ? c1.SequenceEqual(c2) : c1 == c2,
                      c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                      c => c.ToList()));

            // DateTimeOffset conversions
            entity.Property(e => e.JoinedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasMany(e => e.Channels)
                  .WithOne(e => e.Guild)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Users)
                  .WithMany(e => e.Guilds)
                  .UsingEntity(j => j.ToTable("DiscordGuildUsers"));

            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Guild)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Roles)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Invites)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ScheduledEvents)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Bans)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.VoiceSessions)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.PresenceLogs)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Nicknames)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AuditLogs)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Stickers)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordChannel Entity Configurations
        modelBuilder.Entity<DiscordChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            // DateTimeOffset conversions
            entity.Property(e => e.ArchiveTimestamp)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);
            entity.Property(e => e.DeletedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Channels)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Channel)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Users)
                  .WithMany(e => e.Channels)
                  .UsingEntity(j => j.ToTable("DiscordChannelUsers"));

            // Self-referencing for category
            entity.HasOne(e => e.Category)
                  .WithMany(e => e.ChildChannels)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.VoiceSessions)
                  .WithOne(e => e.Channel)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Invites)
                  .WithOne(e => e.Channel)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ScheduledEvents)
                  .WithOne(e => e.Channel)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordUser Entity Configurations
        modelBuilder.Entity<DiscordUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Discriminator).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();
            entity.HasIndex(e => new { e.Username, e.Discriminator }).IsUnique();

            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Author)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Channels)
                  .WithMany(e => e.Users)
                  .UsingEntity(j => j.ToTable("DiscordUserChannels"));

            entity.HasMany(e => e.Guilds)
                  .WithMany(e => e.Users)
                  .UsingEntity(j => j.ToTable("DiscordUserGuilds"));

            entity.HasMany(e => e.Reactions)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.VoiceSessions)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.PresenceLogs)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Bans)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.BansIssued)
                  .WithOne(e => e.BannedBy)
                  .HasForeignKey(e => e.BannedById)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Nicknames)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.InvitesCreated)
                  .WithOne(e => e.Inviter)
                  .HasForeignKey(e => e.InviterId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.ScheduledEventsCreated)
                  .WithOne(e => e.Creator)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.StickersCreated)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.AuditLogsPerformed)
                  .WithOne(e => e.PerformedBy)
                  .HasForeignKey(e => e.PerformedById)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Roles)
                  .WithMany(e => e.Users)
                  .UsingEntity<DiscordGuildUserRole>(
                      l => l.HasOne(e => e.Role).WithMany().HasForeignKey(e => e.RoleId),
                      r => r.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId));
        });

        // DiscordMessage Entity Configurations
        modelBuilder.Entity<DiscordMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Timestamp)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));

            entity.Property(e => e.EditedTimestamp)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.Property(e => e.DeletedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasIndex(e => e.DiscordId).IsUnique();

            entity.HasOne(e => e.Channel)
                  .WithMany(e => e.Messages)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                  .WithMany(e => e.Messages)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Messages)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReferencedMessage)
                  .WithMany(e => e.ReferencedByMessages)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);

            entity.HasMany(e => e.Reactions)
                  .WithOne(e => e.Message)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Attachments)
                  .WithOne(e => e.Message)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Embeds)
                  .WithOne(e => e.Message)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Stickers)
                  .WithMany(e => e.Messages)
                  .UsingEntity<DiscordMessageSticker>(
                      l => l.HasOne(e => e.Sticker).WithMany().HasForeignKey(e => e.StickerId),
                      r => r.HasOne(e => e.Message).WithMany().HasForeignKey(e => e.MessageId));
        });

        // DiscordRole Entity Configurations
        modelBuilder.Entity<DiscordRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            entity.Property(e => e.DeletedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Roles)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordInvite Entity Configurations
        modelBuilder.Entity<DiscordInvite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Code).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();
            entity.HasIndex(e => e.Code).IsUnique();

            entity.Property(e => e.ExpiresAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);
            entity.Property(e => e.RevokedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Invites)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Channel)
                  .WithMany(e => e.Invites)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Inviter)
                  .WithMany(e => e.InvitesCreated)
                  .HasForeignKey(e => e.InviterId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordReaction Entity Configurations
        modelBuilder.Entity<DiscordReaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmojiName).IsRequired();

            // Unique constraint for reaction
            entity.HasIndex(e => new { e.MessageId, e.UserId, e.EmojiName, e.EmojiId }).IsUnique();

            entity.Property(e => e.RemovedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Message)
                  .WithMany(e => e.Reactions)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Reactions)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordVoiceSession Entity Configurations
        modelBuilder.Entity<DiscordVoiceSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired();

            entity.Property(e => e.JoinedAt)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));
            entity.Property(e => e.LeftAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);
            entity.Property(e => e.RequestToSpeakTimestamp)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.VoiceSessions)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Channel)
                  .WithMany(e => e.VoiceSessions)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.VoiceSessions)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // DiscordPresenceLog Entity Configurations
        modelBuilder.Entity<DiscordPresenceLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Timestamp)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));

            entity.HasOne(e => e.User)
                  .WithMany(e => e.PresenceLogs)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.PresenceLogs)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordUserBan Entity Configurations
        modelBuilder.Entity<DiscordUserBan>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.BannedAt)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));
            entity.Property(e => e.UnbannedAt)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Bans)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Bans)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.BannedBy)
                  .WithMany(e => e.BansIssued)
                  .HasForeignKey(e => e.BannedById)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UnbannedBy)
                  .WithMany()
                  .HasForeignKey(e => e.UnbannedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // DiscordScheduledEvent Entity Configurations
        modelBuilder.Entity<DiscordScheduledEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            entity.Property(e => e.ScheduledStartTime)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));
            entity.Property(e => e.ScheduledEndTime)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.ScheduledEvents)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Channel)
                  .WithMany(e => e.ScheduledEvents)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Creator)
                  .WithMany(e => e.ScheduledEventsCreated)
                  .HasForeignKey(e => e.CreatorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordAttachment Entity Configurations
        modelBuilder.Entity<DiscordAttachment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Filename).IsRequired();
            entity.Property(e => e.Url).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            entity.HasOne(e => e.Message)
                  .WithMany(e => e.Attachments)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordEmbed Entity Configurations
        modelBuilder.Entity<DiscordEmbed>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Timestamp)
                  .HasConversion(
                      v => v.HasValue ? v.Value.UtcTicks : (long?)null,
                      v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null);

            entity.HasOne(e => e.Message)
                  .WithMany(e => e.Embeds)
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordSticker Entity Configurations
        modelBuilder.Entity<DiscordSticker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DiscordId).IsRequired();
            entity.Property(e => e.Name).IsRequired();

            entity.HasIndex(e => e.DiscordId).IsUnique();

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Stickers)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.StickersCreated)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordAuditLog Entity Configurations
        modelBuilder.Entity<DiscordAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired();
            entity.Property(e => e.Action).IsRequired();

            entity.Property(e => e.Timestamp)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.AuditLogs)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.PerformedBy)
                  .WithMany(e => e.AuditLogsPerformed)
                  .HasForeignKey(e => e.PerformedById)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // DiscordUserNickname Entity Configurations
        modelBuilder.Entity<DiscordUserNickname>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                  .WithMany(e => e.Nicknames)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Nicknames)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedBy)
                  .WithMany()
                  .HasForeignKey(e => e.ChangedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // DiscordGuildUserRole Join Table Configurations
        modelBuilder.Entity<DiscordGuildUserRole>(entity =>
        {
            entity.HasKey(e => new { e.GuildId, e.UserId, e.RoleId });

            entity.Property(e => e.AssignedAt)
                  .HasConversion(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero));

            entity.HasOne(e => e.Guild)
                  .WithMany()
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedBy)
                  .WithMany()
                  .HasForeignKey(e => e.AssignedById)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // DiscordMessageSticker Join Table Configurations
        modelBuilder.Entity<DiscordMessageSticker>(entity =>
        {
            entity.HasKey(e => new { e.MessageId, e.StickerId });

            entity.HasOne(e => e.Message)
                  .WithMany()
                  .HasForeignKey(e => e.MessageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sticker)
                  .WithMany()
                  .HasForeignKey(e => e.StickerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
