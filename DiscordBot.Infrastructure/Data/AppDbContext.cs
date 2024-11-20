using DiscordBot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Infrastructure.Data;

/// <summary>
/// Represents the Entity Framework Core database context for Discord-related entities.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    /// <summary>
    /// Gets or sets the Messages DbSet.
    /// </summary>
    public DbSet<DiscordMessage> Messages { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    public DbSet<DiscordUser> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Channels DbSet.
    /// </summary>
    public DbSet<DiscordChannel> Channels { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Guilds DbSet.
    /// </summary>
    public DbSet<DiscordGuild> Guilds { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DiscordGuild Entity Configurations
        modelBuilder.Entity<DiscordGuild>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();

            // One-to-Many Relationship: Guild has many Channels
            entity.HasMany(e => e.Channels)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordGuildUser Entity Configurations
        modelBuilder.Entity<DiscordGuildUser>(entity =>
        {
            entity.HasKey(dgu => new { dgu.DiscordGuildId, dgu.UserId });

            entity.HasOne(dgu => dgu.DiscordGuild)
                .WithMany(dg => dg.DiscordGuildUsers)
                .HasForeignKey(dgu => dgu.DiscordGuildId);

            entity.HasOne(dgu => dgu.User)
                .WithMany(du => du.DiscordGuildUsers)
                .HasForeignKey(dgu => dgu.UserId);
        });

        // DiscordChannel Entity Configurations
        modelBuilder.Entity<DiscordChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();

            // One-to-Many Relationship: Channel belongs to a Guild
            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Channels)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many Relationship: Channel has Messages
            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Channel)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordChannelUser Entity Configurations
        modelBuilder.Entity<DiscordChannelUser>(entity =>
        {
            entity.HasKey(dcu => new { dcu.DiscordChannelId, dcu.UserId });

            entity.HasOne(dcu => dcu.DiscordChannel)
                .WithMany(dc => dc.DiscordChannelUsers)
                .HasForeignKey(dcu => dcu.DiscordChannelId);

            entity.HasOne(dcu => dcu.User)
                .WithMany(du => du.DiscordChannelUsers)
                .HasForeignKey(dcu => dcu.UserId);
        });

        // DiscordUser Entity Configurations
        modelBuilder.Entity<DiscordUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Discriminator).IsRequired();

            // Unique Constraint for Username + Discriminator
            entity.HasIndex(e => new { e.Username, e.Discriminator }).IsUnique();

            // Reference to Messages authored by this User
            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Author)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // DiscordMessage Entity Configurations
        modelBuilder.Entity<DiscordMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Timestamp).HasConversion<string>();
            entity.Property(e => e.EditedTimestamp).HasConversion<string>();

            // One-to-Many Relationship: Message belongs to a Channel
            entity.HasOne(e => e.Channel)
                  .WithMany(e => e.Messages)
                  .HasForeignKey(e => e.ChannelId)
                  .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many Relationship: Message has an Author (User)
            entity.HasOne(e => e.Author)
                  .WithMany(e => e.Messages)
                  .HasForeignKey(e => e.AuthorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
