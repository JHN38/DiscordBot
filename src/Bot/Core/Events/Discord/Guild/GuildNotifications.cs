using Discord;
using Discord.WebSocket;

namespace DiscordBot.Bot.Core.Events.Discord.Guild;

public record GuildAvailableNotification(SocketGuild Guild);

public record GuildUnavailableNotification(SocketGuild Guild);

public record GuildUpdatedNotification(SocketGuild Before, SocketGuild After);

public record GuildRoleCreatedNotification(SocketRole Role);

public record GuildRoleUpdatedNotification(SocketRole Before, SocketRole After);

public record GuildRoleDeletedNotification(SocketRole Role);

public record GuildUserUpdatedNotification(SocketUser Before, SocketUser After);

public record GuildUserVoiceStateUpdatedNotification(SocketUser User, SocketVoiceState Before, SocketVoiceState After);

public record GuildUserIsTypingNotification(Cacheable<IUser, ulong> User, Cacheable<IMessageChannel, ulong> Channel);

public record GuildMemberUpdatedNotification(Cacheable<SocketGuildUser, ulong> Before, SocketGuildUser After);

public record GuildMembersDownloadedNotification(SocketGuild Guild);

public record GuildUserJoinedNotification(SocketGuildUser User);

public record GuildUserLeftNotification(SocketGuild Guild, SocketUser User);

public record GuildUserBannedNotification(SocketUser User, SocketGuild Guild);

public record GuildUserUnbannedNotification(SocketUser User, SocketGuild Guild);

public record GuildPresenceUpdateNotification(SocketUser User, SocketPresence OldPresence, SocketPresence NewPresence);

public record GuildScheduledEventNotification(SocketGuildEvent GuildEvent);

public record GuildInviteCreatedNotification(SocketInvite Invite);
