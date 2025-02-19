namespace DiscordBot.Application.Common.Helpers;

/// <summary>
/// Provides a method to convert OpenWeatherApi weather descriptions into corresponding emojis.
/// </summary>
public static class WeatherEmojiConverter
{
    private static readonly Dictionary<string, string> _weatherDescriptionToEmoji = new()
    {
        { "clear sky", "☀️" },
        { "few clouds", "🌤️" },
        { "scattered clouds", "☁️" },
        { "broken clouds", "🌥️" },
        { "overcast clouds", "☁️" },
        { "shower rain", "🌧️" },
        { "rain", "🌦️" },
        { "light rain", "🌦️" },
        { "moderate rain", "🌧️" },
        { "heavy intensity rain", "🌧️" },
        { "very heavy rain", "🌧️" },
        { "extreme rain", "🌧️" },
        { "freezing rain", "❄️" },
        { "light intensity shower rain", "🌧️" },
        { "heavy intensity shower rain", "🌧️" },
        { "ragged shower rain", "🌧️" },
        { "thunderstorm", "⛈️" },
        { "light thunderstorm", "⛈️" },
        { "heavy thunderstorm", "⛈️" },
        { "ragged thunderstorm", "⛈️" },
        { "thunderstorm with light rain", "⛈️" },
        { "thunderstorm with rain", "⛈️" },
        { "thunderstorm with heavy rain", "⛈️" },
        { "snow", "❄️" },
        { "light snow", "❄️" },
        { "heavy snow", "❄️" },
        { "sleet", "🌨️" },
        { "light shower sleet", "🌨️" },
        { "shower sleet", "🌨️" },
        { "light rain and snow", "🌨️" },
        { "rain and snow", "🌨️" },
        { "light shower snow", "🌨️" },
        { "shower snow", "🌨️" },
        { "heavy shower snow", "🌨️" },
        { "mist", "🌫️" },
        { "smoke", "🌫️" },
        { "haze", "🌫️" },
        { "sand, dust whirls", "🌪️" },
        { "fog", "🌫️" },
        { "sand", "🌪️" },
        { "dust", "🌪️" },
        { "volcanic ash", "🌋" },
        { "squalls", "💨" },
        { "tornado", "🌪️" }
    };

    /// <summary>
    /// Converts the given weather description to its corresponding emoji.
    /// </summary>
    /// <param name="description">The weather description from OpenWeatherApi.</param>
    /// <returns>The corresponding emoji if found, otherwise an empty string.</returns>
    public static string ConvertToEmoji(string description)
    {
        if (_weatherDescriptionToEmoji.TryGetValue(description.ToLower(), out var emoji))
        {
            return emoji;
        }

        return description;
    }
}
