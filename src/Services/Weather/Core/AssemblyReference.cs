using System.Reflection;

namespace DiscordBot.Service.Weather.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
