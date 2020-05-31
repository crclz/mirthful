#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base

VOLUME /root/.nuget /root/.nuget # nuget cache

WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Leopard.Seed/Leopard.Seed.csproj", "Leopard.Seed/"]
COPY ["Leopard.Infrastructure.Shell/Leopard.Infrastructure.Shell.csproj", "Leopard.Infrastructure.Shell/"]
COPY ["Leopard.Infrastructure/Leopard.Infrastructure.csproj", "Leopard.Infrastructure/"]
COPY ["Leopard.Domain/Leopard.Domain.csproj", "Leopard.Domain/"]
RUN dotnet restore "Leopard.Seed/Leopard.Seed.csproj"
COPY . .
WORKDIR "/src/Leopard.Seed"
RUN dotnet build "Leopard.Seed.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leopard.Seed.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leopard.Seed.dll"]