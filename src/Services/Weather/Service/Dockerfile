# Weather Service Dockerfile - .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
# Wolverine TCP port
EXPOSE 5002
# Health check HTTP port
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props
COPY ["Directory.Build.props", "."]

# Copy project files
COPY ["src/Services/Weather/Service/DiscordBot.Service.Weather.csproj", "src/Services/Weather/Service/"]
COPY ["src/Services/Weather/Infrastructure/DiscordBot.Service.Weather.Infrastructure.csproj", "src/Services/Weather/Infrastructure/"]
COPY ["src/Services/Weather/Core/DiscordBot.Service.Weather.Core.csproj", "src/Services/Weather/Core/"]
COPY ["src/Contracts/DiscordBot.Contracts.csproj", "src/Contracts/"]

RUN dotnet restore "src/Services/Weather/Service/DiscordBot.Service.Weather.csproj"
COPY . .
WORKDIR "/src/src/Services/Weather/Service"
RUN dotnet build "DiscordBot.Service.Weather.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordBot.Service.Weather.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --chown=app:app --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.Service.Weather.dll"]
