FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["AbySalto.Junior/AbySalto.Junior.csproj", "AbySalto.Junior/"]
RUN dotnet restore "AbySalto.Junior/AbySalto.Junior.csproj"

COPY . .
WORKDIR /src/AbySalto.Junior
RUN dotnet publish "AbySalto.Junior.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AbySalto.Junior.dll"]