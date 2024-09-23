FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Talkative.Worker/Talkative.Worker.csproj", "Talkative.Worker/"]
RUN dotnet restore "Talkative.Worker/Talkative.Worker.csproj"
COPY . .
WORKDIR "/src/Talkative.Worker"
RUN dotnet build "Talkative.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Talkative.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Talkative.Worker.dll"]
