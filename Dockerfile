# 🔥 Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["FiasApiClient/FiasApiClient.csproj", "FiasApiClient/"]
RUN dotnet restore "FiasApiClient/FiasApiClient.csproj"

# Копируем весь исходный код и собираем
COPY . .
WORKDIR "/src/FiasApiClient"
RUN dotnet build "FiasApiClient.csproj" -c Release -o /app/build

# 🔥 Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "FiasApiClient.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 🔥 Этап 3: Runtime (финальный образ)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
# 👆 ВАЖНО: aspnet:8.0, а НЕ runtime:8.0!

WORKDIR /app

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "FiasApiClient.dll"]