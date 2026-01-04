using System.Reflection;

namespace DiscordBot.Bot.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
