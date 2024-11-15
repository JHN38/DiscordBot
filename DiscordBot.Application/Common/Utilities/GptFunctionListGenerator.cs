using System.Reflection;
using System.Text.Json;
using ChatGptNet.Models;
using DiscordBot.Domain.Common;

namespace DiscordBot.Application.Common.Utilities;

public static class GptFunctionListGenerator
{
    public static IEnumerable<ChatGptFunction> GenerateJsonForAllFunctions(Assembly assembly)
    {
        var methods = assembly.GetTypes()
                              .SelectMany(t => t.GetMethods())
                              .Where(m => m.GetCustomAttribute<ChatGptFunctionAttribute>() != null);

        foreach (var method in methods)
        {
            yield return GenerateJsonForFunction(method);
        }
    }

    public static ChatGptFunction GenerateJsonForFunction(MethodInfo methodInfo)
    {
        var functionAttribute = methodInfo.GetCustomAttribute<ChatGptFunctionAttribute>() ?? throw new InvalidOperationException("Method does not have ChatGptFunction attribute.");
        var parametersType = Array.Find(methodInfo.GetParameters(), p => p.ParameterType.GetCustomAttribute<ChatGptPropertiesAttribute>() != null)?.ParameterType
            ?? throw new InvalidOperationException("None of the method properties have the ChatGptProperties attribute.");
        var properties = GetPropertiesJson(parametersType);
        var requiredProperties = GetRequiredProperties(parametersType);

        var parameters = new
        {
            type = "object",
            properties,
            required = requiredProperties
        };

        return new ChatGptFunction
        {
            Name = functionAttribute.Name,
            Description = functionAttribute.Description,
            Parameters = JsonDocument.Parse(JsonSerializer.Serialize(parameters))
        };
    }

    private static object GetPropertiesJson(Type type) =>
        type.GetProperties()
            .Where(p => p.GetCustomAttribute<ChatGptParameterAttribute>() is not null)
            .Select(p =>
            {
                var attr = p.GetCustomAttribute<ChatGptParameterAttribute>() ?? throw new InvalidOperationException("Model does not have ChatGptParameter attribute.");

                return new
                {
                    name = attr.Name,
                    type = GetJsonType(p.PropertyType),
                    description = attr.Description
                };
            })
            .ToDictionary(p => p.name, p => new { p.type, p.description });

    private static string?[] GetRequiredProperties(Type? type) =>
        type?.GetProperties()
            .Where(p => p.GetCustomAttribute<ChatGptParameterAttribute>()?.IsRequired == true)
            .Select(p => p.GetCustomAttribute<ChatGptParameterAttribute>()?.Name)
            .ToArray() ?? [];

    private static string GetJsonType(Type type) =>
        type switch
        {
            _ when type == typeof(string) => "string",
            _ when type == typeof(int) || type == typeof(long) || type == typeof(ulong) => "integer",
            _ when type == typeof(bool) => "boolean",
            _ when type.IsArray || type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()) => "array",
            _ when type.IsClass => "object",
            _ => throw new NotImplementedException($"Type mapping not implemented for {type}")
        };
}