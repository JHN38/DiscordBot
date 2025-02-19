namespace DiscordBot.Application.Common.Interfaces;
public interface IDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
