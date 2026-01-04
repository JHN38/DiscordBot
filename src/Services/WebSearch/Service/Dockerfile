# WebSearch Service Dockerfile - .NET 10
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
# Install curl for health checks
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
USER app
WORKDIR /app
# Wolverine TCP port
EXPOSE 5003
# Health check HTTP port
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Directory.Build.props
COPY ["Directory.Build.props", "."]

# Copy project files
COPY ["src/Services/WebSearch/Service/DiscordBot.Service.WebSearch.csproj", "src/Services/WebSearch/Service/"]
COPY ["src/Services/WebSearch/Infrastructure/DiscordBot.Service.WebSearch.Infrastructure.csproj", "src/Services/WebSearch/Infrastructure/"]
COPY ["src/Services/WebSearch/Core/DiscordBot.Service.WebSearch.Core.csproj", "src/Services/WebSearch/Core/"]
COPY ["src/Contracts/DiscordBot.Contracts.csproj", "src/Contracts/"]

RUN dotnet restore "src/Services/WebSearch/Service/DiscordBot.Service.WebSearch.csproj"
COPY . .
WORKDIR "/src/src/Services/WebSearch/Service"
RUN dotnet build "DiscordBot.Service.WebSearch.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordBot.Service.WebSearch.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --chown=app:app --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordBot.Service.WebSearch.dll"]
