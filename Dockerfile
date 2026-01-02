# =========================
# Base (SDK + EF + pg client)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
WORKDIR /app

RUN apt-get update && \
    apt-get install -y postgresql-client && \
    rm -rf /var/lib/apt/lists/*

RUN dotnet tool install --global dotnet-ef --version 10.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# =========================
# Build
# =========================
FROM base AS build
WORKDIR /src

# Copiar solución y proyectos necesarios
COPY *.sln .
COPY MonitorPrices.API/*.csproj MonitorPrices.API/
COPY MonitorPrices.Domain/*.csproj MonitorPrices.Domain/
COPY MonitorPrices.Repository/*.csproj MonitorPrices.Repository/
COPY MonitorPrices.Services/*.csproj MonitorPrices.Services/

# Restore
RUN dotnet restore MonitorPrices.API/MonitorPrices.API.csproj

# Copiar todo el código
COPY . .

# Publicar API
WORKDIR /src/MonitorPrices.API
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime final
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Puerto típico API
EXPOSE 8080

ENTRYPOINT ["dotnet", "MonitorPrices.API.dll"]
