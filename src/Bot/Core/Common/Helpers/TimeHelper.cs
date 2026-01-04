using NodaTime;
using NodaTime.TimeZones;

namespace DiscordBot.Bot.Core.Common.Helpers;

public static class TimeHelper
{
    public static LocalDateTime ConvertUnixTimeToLocalTime(long unixTime, string countryIsoCode, double? longitude = null)
    {
        Instant instant = Instant.FromUnixTimeSeconds(unixTime);

        var timeZoneProvider = DateTimeZoneProviders.Tzdb;

        // Get all zones for this country
        var countryZones = TzdbDateTimeZoneSource.Default.ZoneLocations?
            .Where(z => z.CountryCode == countryIsoCode)
            .ToList();

        if (countryZones is null or { Count: 0 })
        {
            throw new ArgumentException($"No time zone found for country ISO code: {countryIsoCode}");
        }

        string timeZoneId;

        // If longitude is provided and there are multiple zones, find the closest one
        if (longitude.HasValue && countryZones.Count > 1)
        {
            timeZoneId = countryZones
                .OrderBy(z => Math.Abs(z.Longitude - longitude.Value))
                .First()
                .ZoneId;
        }
        else
        {
            // Fall back to first zone (usually the capital/primary zone)
            timeZoneId = countryZones[0].ZoneId;
        }

        var timeZone = timeZoneProvider[timeZoneId];
        return instant.InZone(timeZone).LocalDateTime;
    }
}
