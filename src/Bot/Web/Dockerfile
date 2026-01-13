# Bot Dockerfile - .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props
COPY ["Directory.Build.props", "."]

# Copy project files
COPY ["src/Bot/Web/DiscordBot.Bot.csproj", "src/Bot/Web/"]
COPY ["src/Bot/Core/DiscordBot.Bot.Core.csproj", "src/Bot/Core/"]
COPY ["src/Contracts/DiscordBot.Contracts.csproj", "src/Contracts/"]

RUN dotnet restore "src/Bot/Web/DiscordBot.Bot.csproj"

COPY . .
WORKDIR "/src/src/Bot/Web"
RUN dotnet build "DiscordBot.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordBot.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --chown=app:app --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.Bot.dll"]
