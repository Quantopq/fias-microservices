# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ClientApiService/ClientApiService.csproj", "ClientApiService/"]
RUN dotnet restore "ClientApiService/ClientApiService.csproj"

COPY . .
WORKDIR "/src/ClientApiService"
RUN dotnet build "ClientApiService.csproj" -c Release -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "ClientApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Этап 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# 👆 ТОЖЕ aspnet:8.0

WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8081
ENTRYPOINT ["dotnet", "ClientApiService.dll"]