# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Manimp.Web/Manimp.Web.csproj", "Manimp.Web/"]
COPY ["Manimp.Services/Manimp.Services.csproj", "Manimp.Services/"]
COPY ["Manimp.Directory/Manimp.Directory.csproj", "Manimp.Directory/"]
COPY ["Manimp.Data/Manimp.Data.csproj", "Manimp.Data/"]
COPY ["Manimp.Auth/Manimp.Auth.csproj", "Manimp.Auth/"]
COPY ["Manimp.Shared/Manimp.Shared.csproj", "Manimp.Shared/"]
COPY ["Manimp.Api/Manimp.Api.csproj", "Manimp.Api/"]

# Restore dependencies
RUN dotnet restore "Manimp.Web/Manimp.Web.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Manimp.Web"
RUN dotnet build "Manimp.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Manimp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user
RUN addgroup --system --gid 1001 dotnet \
    && adduser --system --uid 1001 --gid 1001 dotnet

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory and set permissions
RUN mkdir -p /app/logs && chown -R dotnet:dotnet /app

# Switch to non-root user
USER dotnet

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD wget --spider --quiet http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Manimp.Web.dll"]