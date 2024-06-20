﻿using Discord.WebSocket;
using MediatR;

namespace DiscordBot.Domain.Notifications.Client;

public record GuildMessageReceivedNotification(SocketMessage Message) : INotification;