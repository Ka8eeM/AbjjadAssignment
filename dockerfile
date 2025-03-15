# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["AbjjadAssignment.csproj", "."]
RUN dotnet restore "AbjjadAssignment.csproj"

# Copy all source files, including static data
COPY . .

# Build and publish in Release mode
RUN dotnet publish "AbjjadAssignment.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install libgdiplus for ImageSharp (needed for some image processing operations)
RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Copy static ImageStorage data
COPY --from=build /src/ImageStorage /app/ImageStorage

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Configure healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:5000/health || exit 1

# Set entrypoint
ENTRYPOINT ["dotnet", "AbjjadAssignment.dll"]