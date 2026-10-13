# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY HDNXUdemyAPI/HDNXUdemyAPI.csproj HDNXUdemyAPI/
COPY HDNXUdemy/HDNXUdemyData.csproj HDNXUdemy/
COPY HDNXUdemyModel/HDNXUdemyModel.csproj HDNXUdemyModel/
COPY HDNXUdemyServices/HDNXUdemyServices.csproj HDNXUdemyServices/
RUN dotnet restore HDNXUdemyAPI/HDNXUdemyAPI.csproj

# Copy all source and build
COPY . .
RUN dotnet publish HDNXUdemyAPI/HDNXUdemyAPI.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install healthcheck dependency and prepare writable runtime directories
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && adduser --disabled-password --gecos "" appuser \
    && mkdir -p logs DataProtectionKeys \
    && chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .
USER appuser

ENV ASPNETCORE_URLS=http://+:5194
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5194

HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
  CMD curl -f http://localhost:5194/health/live || exit 1

ENTRYPOINT ["dotnet", "HDNXUdemyAPI.dll"]
