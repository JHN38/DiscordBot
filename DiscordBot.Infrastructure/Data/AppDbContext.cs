﻿using DiscordBot.Domain.Entities;
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

            // Many-to-Many Relationship: Guild has many Users
            entity.HasMany(e => e.Users)
                  .WithMany(e => e.Guilds)
                  .UsingEntity(j => j.ToTable("DiscordGuildUsers"));

            // One-to-Many Relationship: Guild has many Messages
            entity.HasMany(e => e.Messages)
                  .WithOne(e => e.Guild)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);
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

            // Many-to-Many Relationship: Channel has many Users
            entity.HasMany(e => e.Users)
                  .WithMany(e => e.Channels)
                  .UsingEntity(j => j.ToTable("DiscordChannelUsers"));
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

            // Many-to-Many Relationship: User belongs to many Channels
            entity.HasMany(e => e.Channels)
                  .WithMany(e => e.Users)
                  .UsingEntity(j => j.ToTable("DiscordUserChannels"));

            // Many-to-Many Relationship: User belongs to many Guilds
            entity.HasMany(e => e.Guilds)
                  .WithMany(e => e.Users)
                  .UsingEntity(j => j.ToTable("DiscordUserGuilds"));
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

            // One-to-Many Relationship: Message belongs to a Guild
            entity.HasOne(e => e.Guild)
                  .WithMany(e => e.Messages)
                  .HasForeignKey(e => e.GuildId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Optional self-referencing relationship: Message references another message
            entity.HasOne(e => e.ReferencedMessage)
                  .WithMany(e => e.ReferencedByMessages)
                  .HasForeignKey(e => e.ReferencedMessageId)
                  .OnDelete(DeleteBehavior.Restrict)
                  .IsRequired(false);
        });
    }
}
