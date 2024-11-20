using Discord;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Entities;

namespace DiscordBot.Application.Discord.Messages;

public interface IDiscordMessageRepository : IRepository<ulong, DiscordMessage>
{
    Task AddWithDependenciesAsync(IMessage message, IChannel channel, IGuild guild, IUser author, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetFirstMessagesAsync(int numberOfMessages, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetLastMessagesAsync(int numberOfMessages, CancellationToken cancellationToken = default);
    Task<IEnumerable<DiscordMessage>> GetMatchingMessagesAsync(string needle, CancellationToken cancellationToken = default);
}
