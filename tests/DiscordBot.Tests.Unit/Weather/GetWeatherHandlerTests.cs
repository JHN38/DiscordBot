using DiscordBot.Contracts.Weather;
using DiscordBot.Service.Weather.Core.Interfaces;
using DiscordBot.Service.Weather.Handlers;
using FluentAssertions;
using NSubstitute;
using Xunit;
using DomainEntities = DiscordBot.Service.Weather.Core.Domain;
using DomainEnums = DiscordBot.Service.Weather.Core.Domain.Enums;

namespace DiscordBot.Tests.Unit.Weather;

/// <summary>
/// Unit tests for GetWeatherHandler.
/// Tests the handler's logic for parsing weather types, null handling, and response mapping.
/// </summary>
[Trait("Category", "Unit")]
public sealed class GetWeatherHandlerTests
{
    private readonly IWeatherService _weatherService = Substitute.For<IWeatherService>();
    private readonly GetWeatherHandler _handler;

    public GetWeatherHandlerTests()
    {
        _handler = new GetWeatherHandler(_weatherService);
    }

    [Fact]
    public async Task Invalid_weather_type_defaults_to_Weather()
    {
        // Arrange - GetWeatherQuery(Location, Type = "Weather", Units = "metric")
        var query = new GetWeatherQuery("London", "garbage", "metric");
        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns((DomainEntities.WeatherResponse?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert - Should have called with Weather type (default)
        await _weatherService.Received(1).GetWeatherAsync(
            DomainEnums.WeatherRequestType.Weather,
            "London",
            "metric",
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("weather")]
    [InlineData("Weather")]
    [InlineData("WEATHER")]
    public async Task Valid_weather_type_is_parsed_correctly(string weatherType)
    {
        // Arrange
        var query = new GetWeatherQuery("London", weatherType, "metric");
        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns((DomainEntities.WeatherResponse?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _weatherService.Received(1).GetWeatherAsync(
            DomainEnums.WeatherRequestType.Weather,
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("forecast")]
    [InlineData("Forecast")]
    [InlineData("FORECAST")]
    public async Task Forecast_type_is_parsed_correctly(string forecastType)
    {
        // Arrange
        var query = new GetWeatherQuery("London", forecastType, "metric");
        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns((DomainEntities.WeatherResponse?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _weatherService.Received(1).GetWeatherAsync(
            DomainEnums.WeatherRequestType.Forecast,
            Arg.Any<string>(),
            Arg.Any<string?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Null_service_response_returns_null()
    {
        // Arrange
        var query = new GetWeatherQuery("UnknownCity");
        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns((DomainEntities.WeatherResponse?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Response_items_are_mapped_to_contract()
    {
        // Arrange
        var query = new GetWeatherQuery("London");
        var now = DateTime.UtcNow;
        var domainResponse = new DomainEntities.WeatherResponse(
        [
            new DomainEntities.WeatherResponseItem(
                Location: new DomainEntities.Location("London", "GB", -0.1257, 51.5085, 0),
                Title: "Clear",
                Description: "clear sky",
                IconUrl: new Uri("https://openweathermap.org/img/w/01d.png"),
                Temperature: new DomainEntities.Temperature(15.5, 14.0, 12.0, 18.0),
                Pressure: 1013,
                Humidity: 70,
                Visibility: 10000,
                Clouds: 5,
                DateTime: now,
                Sunrise: 1609459200L,
                Sunset: 1609491600L,
                Wind: new DomainEntities.Wind(5.5, 180))
        ]);

        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns(domainResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(1);

        var item = result.Items[0];
        item.Location.City.Should().Be("London");
        item.Location.Country.Should().Be("GB");
        item.Title.Should().Be("Clear");
        item.Description.Should().Be("clear sky");
        item.Temperature.Temp.Should().Be(15.5);
        item.Humidity.Should().Be(70);
        item.Wind.Speed.Should().Be(5.5);
    }

    [Fact]
    public async Task Location_is_passed_to_service()
    {
        // Arrange
        var query = new GetWeatherQuery("Tokyo", "Weather", "imperial");
        _weatherService.GetWeatherAsync(
                Arg.Any<DomainEnums.WeatherRequestType>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<CancellationToken>())
            .Returns((DomainEntities.WeatherResponse?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _weatherService.Received(1).GetWeatherAsync(
            Arg.Any<DomainEnums.WeatherRequestType>(),
            "Tokyo",
            "imperial",
            Arg.Any<CancellationToken>());
    }
}
