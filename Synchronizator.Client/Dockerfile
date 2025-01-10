FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Synchronizator.Client/Synchronizator.Client.csproj", "Synchronizator.Client/"]
RUN dotnet restore "Synchronizator.Client/Synchronizator.Client.csproj"
COPY . .
WORKDIR "/src/Synchronizator.Client"
RUN dotnet build "Synchronizator.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Synchronizator.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Synchronizator.Client.dll"]
