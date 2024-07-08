using NodaTime;
using NodaTime.TimeZones;

namespace DiscordBot.Application.Common.Helpers;

public static class TimeHelper
{
    /// <summary>
    /// Converts Unix time to local time of a specified country using its ISO code and returns the formatted time string.
    /// </summary>
    /// <param name="unixTime">The Unix time to convert.</param>
    /// <param name="countryIsoCode">The ISO code of the country to get the local time for.</param>
    /// <returns>The local time as a formatted string.</returns>
    public static LocalDateTime ConvertUnixTimeToLocalTime(long unixTime, string countryIsoCode)
    {
        Instant instant = Instant.FromUnixTimeSeconds(unixTime);

        var timeZoneProvider = DateTimeZoneProviders.Tzdb;
        var timeZoneId = (TzdbDateTimeZoneSource.Default.ZoneLocations?
            .FirstOrDefault(z => z.CountryCode == countryIsoCode)?.ZoneId)
            ?? throw new ArgumentException($"No time zone found for country ISO code: {countryIsoCode}");

        var timeZone = timeZoneProvider[timeZoneId];
        return instant.InZone(timeZone).LocalDateTime;
    }
}
