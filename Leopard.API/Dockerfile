#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
VOLUME /root/.nuget /root/.nuget
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
VOLUME /root/.nuget /root/.nuget
WORKDIR /src
COPY ["Leopard.API/Leopard.API.csproj", "Leopard.API/"]
COPY ["Leopard.Infrastructure.Shell/Leopard.Infrastructure.Shell.csproj", "Leopard.Infrastructure.Shell/"]
COPY ["Leopard.Infrastructure/Leopard.Infrastructure.csproj", "Leopard.Infrastructure/"]
COPY ["Leopard.Domain/Leopard.Domain.csproj", "Leopard.Domain/"]
RUN dotnet restore "Leopard.API/Leopard.API.csproj"
COPY . .
WORKDIR "/src/Leopard.API"
RUN dotnet build "Leopard.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Leopard.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Leopard.API.dll"]