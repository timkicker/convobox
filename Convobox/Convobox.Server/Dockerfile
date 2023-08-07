FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Convobox.Server/Convobox.Server.csproj", "Convobox.Server/"]
RUN dotnet restore "Convobox.Server/Convobox.Server.csproj"
COPY . .
WORKDIR "/src/Convobox.Server"
RUN dotnet build "Convobox.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Convobox.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Convobox.Server.dll"]
