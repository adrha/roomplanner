#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RoomPlanner.App/RoomPlanner.App.csproj", "RoomPlanner.App/"]
COPY ["RoomPlanner.Core/RoomPlanner.Core.csproj", "RoomPlanner.Core/"]
COPY ["RoomPlanner.Business/RoomPlanner.Business.csproj", "RoomPlanner.Business/"]
COPY ["RoomPlanner.Infrastructure/RoomPlanner.Infrastructure.csproj", "RoomPlanner.Infrastructure/"]

RUN dotnet restore "RoomPlanner.App/RoomPlanner.App.csproj"
COPY . .
WORKDIR "/src/RoomPlanner.App"
RUN dotnet build "RoomPlanner.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RoomPlanner.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RoomPlanner.App.dll"]