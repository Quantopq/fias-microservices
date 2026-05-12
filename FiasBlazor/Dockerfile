FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FiasBlazor/FiasBlazor.csproj", "FiasBlazor/"]
RUN dotnet restore "FiasBlazor/FiasBlazor.csproj"

COPY . .
WORKDIR "/src/FiasBlazor"
RUN dotnet build "FiasBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FiasBlazor.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "FiasBlazor.dll"]
