FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApps/WebHealthStatus/WebHealthStatus.csproj", "WebApps/WebHealthStatus/"]
RUN dotnet restore "WebApps/WebHealthStatus/WebHealthStatus.csproj"
COPY . .
WORKDIR "/src/WebApps/WebHealthStatus"
RUN dotnet build "WebHealthStatus.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebHealthStatus.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebHealthStatus.dll"]
