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

# Copiar proyecto ControlPanel
COPY ControlPanel/ControlPanel.csproj ControlPanel/
RUN dotnet restore ControlPanel/ControlPanel.csproj

COPY ControlPanel/ ControlPanel/
WORKDIR /src/ControlPanel

# Publicar Blazor Server
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer puerto de Blazor
EXPOSE 5001

ENTRYPOINT ["dotnet", "ControlPanel.dll"]
