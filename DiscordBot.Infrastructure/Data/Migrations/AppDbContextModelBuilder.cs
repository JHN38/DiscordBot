﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace DiscordBot.Infrastructure.Data.Migrations
{
    public partial class AppDbContextModel
    {
        private AppDbContextModel()
            : base(skipDetectChanges: false, modelId: new Guid("13fb5084-b370-48b5-8d57-2f6876d50472"), entityTypeCount: 6)
        {
        }

        partial void Initialize()
        {
            var discordChannel = DiscordChannelEntityType.Create(this);
            var discordGuild = DiscordGuildEntityType.Create(this);
            var discordMessage = DiscordMessageEntityType.Create(this);
            var discordUser = DiscordUserEntityType.Create(this);
            var discordChannelDiscordUser = DiscordChannelDiscordUserEntityType.Create(this);
            var discordGuildDiscordUser = DiscordGuildDiscordUserEntityType.Create(this);

            DiscordChannelEntityType.CreateForeignKey1(discordChannel, discordGuild);
            DiscordMessageEntityType.CreateForeignKey1(discordMessage, discordUser);
            DiscordMessageEntityType.CreateForeignKey2(discordMessage, discordChannel);
            DiscordMessageEntityType.CreateForeignKey3(discordMessage, discordGuild);
            DiscordMessageEntityType.CreateForeignKey4(discordMessage, discordMessage);
            DiscordChannelDiscordUserEntityType.CreateForeignKey1(discordChannelDiscordUser, discordChannel);
            DiscordChannelDiscordUserEntityType.CreateForeignKey2(discordChannelDiscordUser, discordUser);
            DiscordGuildDiscordUserEntityType.CreateForeignKey1(discordGuildDiscordUser, discordGuild);
            DiscordGuildDiscordUserEntityType.CreateForeignKey2(discordGuildDiscordUser, discordUser);

            DiscordChannelEntityType.CreateSkipNavigation1(discordChannel, discordUser, discordChannelDiscordUser);
            DiscordGuildEntityType.CreateSkipNavigation1(discordGuild, discordUser, discordGuildDiscordUser);
            DiscordUserEntityType.CreateSkipNavigation1(discordUser, discordChannel, discordChannelDiscordUser);
            DiscordUserEntityType.CreateSkipNavigation2(discordUser, discordGuild, discordGuildDiscordUser);

            DiscordChannelEntityType.CreateAnnotations(discordChannel);
            DiscordGuildEntityType.CreateAnnotations(discordGuild);
            DiscordMessageEntityType.CreateAnnotations(discordMessage);
            DiscordUserEntityType.CreateAnnotations(discordUser);
            DiscordChannelDiscordUserEntityType.CreateAnnotations(discordChannelDiscordUser);
            DiscordGuildDiscordUserEntityType.CreateAnnotations(discordGuildDiscordUser);

            AddAnnotation("ProductVersion", "9.0.0");
            AddAnnotation("Proxies:ChangeTracking", false);
            AddAnnotation("Proxies:CheckEquality", false);
            AddAnnotation("Proxies:LazyLoading", true);
        }
    }
}
