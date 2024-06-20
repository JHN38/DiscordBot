var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DiscordBot_Bot>("discordbot-bot");

builder.Build().Run();
