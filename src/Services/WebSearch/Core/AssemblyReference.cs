using System.Reflection;

namespace DiscordBot.Service.WebSearch.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
