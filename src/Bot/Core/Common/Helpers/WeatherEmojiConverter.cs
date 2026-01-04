namespace DiscordBot.Bot.Core.Common.Helpers;

public static class WeatherEmojiConverter
{
    private static readonly Dictionary<string, string> _weatherDescriptionToEmoji = new()
    {
        { "CLEAR SKY", "☀️" },
        { "FEW CLOUDS", "🌤️" },
        { "SCATTERED CLOUDS", "☁️" },
        { "BROKEN CLOUDS", "🌥️" },
        { "OVERCAST CLOUDS", "☁️" },
        { "SHOWER RAIN", "🌧️" },
        { "RAIN", "🌦️" },
        { "LIGHT RAIN", "🌦️" },
        { "MODERATE RAIN", "🌧️" },
        { "HEAVY INTENSITY RAIN", "🌧️" },
        { "VERY HEAVY RAIN", "🌧️" },
        { "EXTREME RAIN", "🌧️" },
        { "FREEZING RAIN", "❄️" },
        { "LIGHT INTENSITY SHOWER RAIN", "🌧️" },
        { "HEAVY INTENSITY SHOWER RAIN", "🌧️" },
        { "RAGGED SHOWER RAIN", "🌧️" },
        { "THUNDERSTORM", "⛈️" },
        { "LIGHT THUNDERSTORM", "⛈️" },
        { "HEAVY THUNDERSTORM", "⛈️" },
        { "RAGGED THUNDERSTORM", "⛈️" },
        { "THUNDERSTORM WITH LIGHT RAIN", "⛈️" },
        { "THUNDERSTORM WITH RAIN", "⛈️" },
        { "THUNDERSTORM WITH HEAVY RAIN", "⛈️" },
        { "SNOW", "❄️" },
        { "LIGHT SNOW", "❄️" },
        { "HEAVY SNOW", "❄️" },
        { "SLEET", "🌨️" },
        { "LIGHT SHOWER SLEET", "🌨️" },
        { "SHOWER SLEET", "🌨️" },
        { "LIGHT RAIN AND SNOW", "🌨️" },
        { "RAIN AND SNOW", "🌨️" },
        { "LIGHT SHOWER SNOW", "🌨️" },
        { "SHOWER SNOW", "🌨️" },
        { "HEAVY SHOWER SNOW", "🌨️" },
        { "MIST", "🌫️" },
        { "SMOKE", "🌫️" },
        { "HAZE", "🌫️" },
        { "SAND, DUST WHIRLS", "🌪️" },
        { "FOG", "🌫️" },
        { "SAND", "🌪️" },
        { "DUST", "🌪️" },
        { "VOLCANIC ASH", "🌋" },
        { "SQUALLS", "💨" },
        { "TORNADO", "🌪️" }
    };

    public static string ConvertToEmoji(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        if (_weatherDescriptionToEmoji.TryGetValue(description.ToUpperInvariant(), out var emoji))
        {
            return emoji;
        }

        return description;
    }
}
