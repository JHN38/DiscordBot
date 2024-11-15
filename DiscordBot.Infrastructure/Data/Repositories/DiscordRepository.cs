using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Application.Common.Interfaces;
using DiscordBot.Domain.Common;

namespace DiscordBot.Infrastructure.Data.Repositories;
public class DiscordRepository<TEntity>(IAppDbContext context) : RepositoryBase<TEntity>(context)
    where TEntity : class
{

}
