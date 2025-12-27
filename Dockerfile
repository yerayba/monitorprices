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

# Copiar solución y proyectos
COPY MonitorPrices.sln ./

COPY MonitorPrices.API/MonitorPrices.API.csproj MonitorPrices.API/
COPY MonitorPrices.Domain/MonitorPrices.Domain.csproj MonitorPrices.Domain/
COPY MonitorPrices.Repository/MonitorPrices.Repository.csproj MonitorPrices.Repository/
COPY MonitorPrices.Services/MonitorPrices.Services.csproj MonitorPrices.Services/

# Restaurar dependencias
RUN dotnet restore MonitorPrices.sln

# Copiar el resto del código
COPY . .

# Compilar API
WORKDIR /src/MonitorPrices.API
RUN dotnet build -c Release -o /app/build

# =========================
# Publish
# =========================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime final
# =========================
FROM base AS final
WORKDIR /app

# Copiar salida publicada
COPY --from=publish /app/publish .

# Ejecutar directamente la aplicación
ENTRYPOINT ["dotnet", "MonitorPrices.API.dll"]
