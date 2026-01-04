namespace DiscordBot.Service.RecordKeeping.Core.Domain.Enums;

/// <summary>
/// Discord channel types. Values match Discord API.
/// </summary>
public enum ChannelType
{
    Text = 0,
    DM = 1,
    Voice = 2,
    GroupDM = 3,
    Category = 4,
    News = 5,
    NewsThread = 10,
    PublicThread = 11,
    PrivateThread = 12,
    Stage = 13,
    GuildDirectory = 14,
    Forum = 15,
    Media = 16
}

/// <summary>
/// Discord message types. Values match Discord API.
/// </summary>
public enum MessageType
{
    Default = 0,
    RecipientAdd = 1,
    RecipientRemove = 2,
    Call = 3,
    ChannelNameChange = 4,
    ChannelIconChange = 5,
    ChannelPinnedMessage = 6,
    UserJoin = 7,
    GuildBoost = 8,
    GuildBoostTier1 = 9,
    GuildBoostTier2 = 10,
    GuildBoostTier3 = 11,
    ChannelFollowAdd = 12,
    GuildDiscoveryDisqualified = 14,
    GuildDiscoveryRequalified = 15,
    GuildDiscoveryGracePeriodInitialWarning = 16,
    GuildDiscoveryGracePeriodFinalWarning = 17,
    ThreadCreated = 18,
    Reply = 19,
    ChatInputCommand = 20,
    ThreadStarterMessage = 21,
    GuildInviteReminder = 22,
    ContextMenuCommand = 23,
    AutoModerationAction = 24,
    RoleSubscriptionPurchase = 25,
    InteractionPremiumUpsell = 26,
    StageStart = 27,
    StageEnd = 28,
    StageSpeaker = 29,
    StageTopic = 31,
    GuildApplicationPremiumSubscription = 32
}

/// <summary>
/// User presence status. Values match Discord API.
/// </summary>
public enum PresenceStatus
{
    Online = 0,
    Idle = 1,
    DoNotDisturb = 2,
    Offline = 3,
    Invisible = 4
}

/// <summary>
/// Activity type for user presence. Values match Discord API.
/// </summary>
public enum ActivityType
{
    Playing = 0,
    Streaming = 1,
    Listening = 2,
    Watching = 3,
    Custom = 4,
    Competing = 5
}

/// <summary>
/// Scheduled event status. Values match Discord API.
/// </summary>
public enum ScheduledEventStatus
{
    Scheduled = 1,
    Active = 2,
    Completed = 3,
    Cancelled = 4
}

/// <summary>
/// Scheduled event privacy level. Values match Discord API.
/// </summary>
public enum ScheduledEventPrivacyLevel
{
    GuildOnly = 2
}

/// <summary>
/// Scheduled event entity type. Values match Discord API.
/// </summary>
public enum ScheduledEventEntityType
{
    StageInstance = 1,
    Voice = 2,
    External = 3
}

/// <summary>
/// Sticker format type. Values match Discord API.
/// </summary>
public enum StickerFormatType
{
    Png = 1,
    Apng = 2,
    Lottie = 3,
    Gif = 4
}

/// <summary>
/// Guild verification level. Values match Discord API.
/// </summary>
public enum VerificationLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    VeryHigh = 4
}

/// <summary>
/// Guild default message notification level. Values match Discord API.
/// </summary>
public enum DefaultMessageNotifications
{
    AllMessages = 0,
    OnlyMentions = 1
}

/// <summary>
/// Guild explicit content filter level. Values match Discord API.
/// </summary>
public enum ExplicitContentFilter
{
    Disabled = 0,
    MembersWithoutRoles = 1,
    AllMembers = 2
}
