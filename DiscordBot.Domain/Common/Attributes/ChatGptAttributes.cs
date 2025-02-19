namespace DiscordBot.Domain.Common;

/// <summary>
/// Attribute to mark a method as a ChatGPT function.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ChatGptFunctionAttribute(string name, string? description) : Attribute
{
    /// <summary>
    /// Gets or sets the name of the ChatGPT function.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the description of the ChatGPT function.
    /// </summary>
    public string? Description { get; set; } = description;
}

/// <summary>
/// Defines an attribute to be used on classes to specify custom properties for ChatGPT.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ChatGptPropertiesAttribute : Attribute;

/// <summary>
/// Attribute to mark a property as a parameter for a ChatGPT function.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public class ChatGptParameterAttribute(string name, string? description, bool isRequired) : Attribute
{
    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the description of the parameter.
    /// </summary>
    public string? Description { get; set; } = description;

    /// <summary>
    /// Gets or sets a value indicating whether the parameter is required.
    /// </summary>
    public bool IsRequired { get; set; } = isRequired;
}