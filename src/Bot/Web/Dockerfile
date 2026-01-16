FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
# Install curl for health checks
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app/src

# Copy solution and build configuration files
COPY ["NuGet.Config", "."]
COPY ["Directory.Build.props", "."]
COPY ["DiscordBot.slnx", "."]

# Copy all project files for restore caching
COPY ["src/Bot/Core/DiscordBot.Bot.Core.csproj", "src/Bot/Core/"]
COPY ["src/Bot/Web/DiscordBot.Bot.csproj", "src/Bot/Web/"]
COPY ["src/Contracts/DiscordBot.Contracts.csproj", "src/Contracts/"]
COPY ["src/Services/RecordKeeping/Core/DiscordBot.Service.RecordKeeping.Core.csproj", "src/Services/RecordKeeping/Core/"]
COPY ["src/Services/RecordKeeping/Persistence/DiscordBot.Service.RecordKeeping.Persistence.csproj", "src/Services/RecordKeeping/Persistence/"]
COPY ["src/Services/RecordKeeping/Service/DiscordBot.Service.RecordKeeping.csproj", "src/Services/RecordKeeping/Service/"]
COPY ["src/Services/Weather/Core/DiscordBot.Service.Weather.Core.csproj", "src/Services/Weather/Core/"]
COPY ["src/Services/Weather/Infrastructure/DiscordBot.Service.Weather.Infrastructure.csproj", "src/Services/Weather/Infrastructure/"]
COPY ["src/Services/Weather/Service/DiscordBot.Service.Weather.csproj", "src/Services/Weather/Service/"]
COPY ["src/Services/WebSearch/Core/DiscordBot.Service.WebSearch.Core.csproj", "src/Services/WebSearch/Core/"]
COPY ["src/Services/WebSearch/Infrastructure/DiscordBot.Service.WebSearch.Infrastructure.csproj", "src/Services/WebSearch/Infrastructure/"]
COPY ["src/Services/WebSearch/Service/DiscordBot.Service.WebSearch.csproj", "src/Services/WebSearch/Service/"]

RUN dotnet restore "src/Bot/Web/DiscordBot.Bot.csproj" \
    --configfile NuGet.Config

COPY . .
RUN dotnet publish "src/Bot/Web/DiscordBot.Bot.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app

COPY --chown=app:app --from=build "/app/publish" .

USER app

EXPOSE 8080
EXPOSE 8081

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "DiscordBot.Bot.dll"]
