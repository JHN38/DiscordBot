using System.Reflection;

namespace DiscordBot.Service.RecordKeeping.Core;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
