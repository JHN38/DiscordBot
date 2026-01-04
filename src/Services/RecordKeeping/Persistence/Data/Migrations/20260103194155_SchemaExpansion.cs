using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Service.RecordKeeping.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaExpansion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccentColor",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GlobalName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBot",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWebhook",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Locale",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PremiumType",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicFlags",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ApplicationId",
                table: "Messages",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedAt",
                table: "Messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flags",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "InteractionId",
                table: "Messages",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTTS",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MentionedChannelIds",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MentionedEveryone",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MentionedRoleIds",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentionedUserIds",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageType",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ThreadId",
                table: "Messages",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WebhookId",
                table: "Messages",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AfkChannelId",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AfkTimeout",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApproximateMemberCount",
                table: "Guilds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApproximatePresenceCount",
                table: "Guilds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultMessageNotifications",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExplicitContentFilter",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconUrl",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Guilds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNsfw",
                table: "Guilds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "JoinedAt",
                table: "Guilds",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxMembers",
                table: "Guilds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxVideoChannelUsers",
                table: "Guilds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberCount",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OwnerId",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PreferredLocale",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PremiumSubscriptionCount",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PremiumTier",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PublicUpdatesChannelId",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RulesChannelId",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SystemChannelId",
                table: "Guilds",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VanityUrlCode",
                table: "Guilds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerificationLevel",
                table: "Guilds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "WidgetEnabled",
                table: "Guilds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ArchiveTimestamp",
                table: "Channels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvailableTags",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Bitrate",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChannelType",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultAutoArchiveDuration",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultReactionEmoji",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultSortOrder",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultThreadRateLimitPerUser",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedAt",
                table: "Channels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Channels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Channels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNsfw",
                table: "Channels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ParentId",
                table: "Channels",
                type: "decimal(20,0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionOverwrites",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RateLimitPerUser",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlowModeInterval",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserLimit",
                table: "Channels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProxyUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSpoiler = table.Column<bool>(type: "bit", nullable: false),
                    IsVoiceMessage = table.Column<bool>(type: "bit", nullable: false),
                    DurationSecs = table.Column<float>(type: "real", nullable: true),
                    Waveform = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityDiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: true),
                    PerformedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Embeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: true),
                    Color = table.Column<int>(type: "int", nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterIconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorIconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Embeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Embeds_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaxAge = table.Column<int>(type: "int", nullable: true),
                    MaxUses = table.Column<int>(type: "int", nullable: true),
                    Uses = table.Column<int>(type: "int", nullable: false),
                    IsTemporary = table.Column<bool>(type: "bit", nullable: false),
                    ExpiresAt = table.Column<long>(type: "bigint", nullable: true),
                    TargetType = table.Column<int>(type: "int", nullable: true),
                    TargetUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    TargetApplicationId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<long>(type: "bigint", nullable: true),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    InviterId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invites_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invites_Users_InviterId",
                        column: x => x.InviterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PresenceLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    ClientStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityType = table.Column<int>(type: "int", nullable: true),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActivityUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomStatusEmoji = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresenceLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresenceLogs_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PresenceLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmojiName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmojiId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    IsCustomEmoji = table.Column<bool>(type: "bit", nullable: false),
                    IsAnimated = table.Column<bool>(type: "bit", nullable: false),
                    IsBurst = table.Column<bool>(type: "bit", nullable: false),
                    RemovedAt = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    IsHoisted = table.Column<bool>(type: "bit", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Permissions = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IsManaged = table.Column<bool>(type: "bit", nullable: false),
                    IsMentionable = table.Column<bool>(type: "bit", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnicodeEmoji = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<long>(type: "bigint", nullable: true),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduledStartTime = table.Column<long>(type: "bigint", nullable: false),
                    ScheduledEndTime = table.Column<long>(type: "bigint", nullable: true),
                    PrivacyLevel = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    EntityMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserCount = table.Column<int>(type: "int", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledEvents_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ScheduledEvents_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledEvents_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Stickers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormatType = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    SortValue = table.Column<int>(type: "int", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DiscordId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stickers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stickers_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stickers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserBans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannedAt = table.Column<long>(type: "bigint", nullable: false),
                    UnbannedAt = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    BannedById = table.Column<int>(type: "int", nullable: true),
                    UnbannedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBans_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBans_Users_BannedById",
                        column: x => x.BannedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBans_Users_UnbannedById",
                        column: x => x.UnbannedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserNicknames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousNickname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    ChangedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNicknames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNicknames_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserNicknames_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserNicknames_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoiceSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinedAt = table.Column<long>(type: "bigint", nullable: false),
                    LeftAt = table.Column<long>(type: "bigint", nullable: true),
                    IsSelfMuted = table.Column<bool>(type: "bit", nullable: false),
                    IsSelfDeafened = table.Column<bool>(type: "bit", nullable: false),
                    IsServerMuted = table.Column<bool>(type: "bit", nullable: false),
                    IsServerDeafened = table.Column<bool>(type: "bit", nullable: false),
                    IsStreaming = table.Column<bool>(type: "bit", nullable: false),
                    IsVideoEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsSuppressed = table.Column<bool>(type: "bit", nullable: false),
                    RequestToSpeakTimestamp = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoiceSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoiceSessions_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoiceSessions_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoiceSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildUserRoles",
                columns: table => new
                {
                    GuildId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<long>(type: "bigint", nullable: false),
                    AssignedById = table.Column<int>(type: "int", nullable: true),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildUserRoles", x => new { x.GuildId, x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_GuildUserRoles_Guilds_GuildId",
                        column: x => x.GuildId,
                        principalTable: "Guilds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildUserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildUserRoles_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GuildUserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageStickers",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    StickerId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageStickers", x => new { x.MessageId, x.StickerId });
                    table.ForeignKey(
                        name: "FK_MessageStickers_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageStickers_Stickers_StickerId",
                        column: x => x.StickerId,
                        principalTable: "Stickers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CategoryId",
                table: "Channels",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_DiscordId",
                table: "Attachments",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_MessageId",
                table: "Attachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_GuildId",
                table: "AuditLogs",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedById",
                table: "AuditLogs",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_Embeds_MessageId",
                table: "Embeds",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildUserRoles_AssignedById",
                table: "GuildUserRoles",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_GuildUserRoles_RoleId",
                table: "GuildUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildUserRoles_UserId",
                table: "GuildUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_ChannelId",
                table: "Invites",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_Code",
                table: "Invites",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_DiscordId",
                table: "Invites",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invites_GuildId",
                table: "Invites",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InviterId",
                table: "Invites",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageStickers_StickerId",
                table: "MessageStickers",
                column: "StickerId");

            migrationBuilder.CreateIndex(
                name: "IX_PresenceLogs_GuildId",
                table: "PresenceLogs",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_PresenceLogs_UserId",
                table: "PresenceLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_MessageId_UserId_EmojiName_EmojiId",
                table: "Reactions",
                columns: new[] { "MessageId", "UserId", "EmojiName", "EmojiId" },
                unique: true,
                filter: "[EmojiId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId",
                table: "Reactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_DiscordId",
                table: "Roles",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_GuildId",
                table: "Roles",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_ChannelId",
                table: "ScheduledEvents",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_CreatorId",
                table: "ScheduledEvents",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_DiscordId",
                table: "ScheduledEvents",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledEvents_GuildId",
                table: "ScheduledEvents",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Stickers_DiscordId",
                table: "Stickers",
                column: "DiscordId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stickers_GuildId",
                table: "Stickers",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_Stickers_UserId",
                table: "Stickers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_BannedById",
                table: "UserBans",
                column: "BannedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_GuildId",
                table: "UserBans",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UnbannedById",
                table: "UserBans",
                column: "UnbannedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserId",
                table: "UserBans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNicknames_ChangedById",
                table: "UserNicknames",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserNicknames_GuildId",
                table: "UserNicknames",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNicknames_UserId",
                table: "UserNicknames",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceSessions_ChannelId",
                table: "VoiceSessions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceSessions_GuildId",
                table: "VoiceSessions",
                column: "GuildId");

            migrationBuilder.CreateIndex(
                name: "IX_VoiceSessions_UserId",
                table: "VoiceSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Channels_CategoryId",
                table: "Channels",
                column: "CategoryId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Channels_CategoryId",
                table: "Channels");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Embeds");

            migrationBuilder.DropTable(
                name: "GuildUserRoles");

            migrationBuilder.DropTable(
                name: "Invites");

            migrationBuilder.DropTable(
                name: "MessageStickers");

            migrationBuilder.DropTable(
                name: "PresenceLogs");

            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "ScheduledEvents");

            migrationBuilder.DropTable(
                name: "UserBans");

            migrationBuilder.DropTable(
                name: "UserNicknames");

            migrationBuilder.DropTable(
                name: "VoiceSessions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Stickers");

            migrationBuilder.DropIndex(
                name: "IX_Channels_CategoryId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AccentColor",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GlobalName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBot",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsWebhook",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Locale",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PremiumType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicFlags",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "InteractionId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsTTS",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MentionedChannelIds",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MentionedEveryone",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MentionedRoleIds",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MentionedUserIds",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "WebhookId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "AfkChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "AfkTimeout",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ApproximateMemberCount",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ApproximatePresenceCount",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "DefaultMessageNotifications",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ExplicitContentFilter",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "Features",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IconUrl",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "IsNsfw",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "MaxMembers",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "MaxVideoChannelUsers",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "PreferredLocale",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "PremiumSubscriptionCount",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "PremiumTier",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "PublicUpdatesChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "RulesChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "SystemChannelId",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "VanityUrlCode",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "VerificationLevel",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "WidgetEnabled",
                table: "Guilds");

            migrationBuilder.DropColumn(
                name: "ArchiveTimestamp",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AvailableTags",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Bitrate",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ChannelType",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DefaultAutoArchiveDuration",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DefaultReactionEmoji",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DefaultSortOrder",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DefaultThreadRateLimitPerUser",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "IsNsfw",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "PermissionOverwrites",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "RateLimitPerUser",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "SlowModeInterval",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "UserLimit",
                table: "Channels");
        }
    }
}
