# 1. Base Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8084
ENV ASPNETCORE_HTTP_PORTS=8084

# 2. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file first for Docker layer caching
COPY ["MiniWalletSystemApi/MiniWalletSystemApi.csproj", "MiniWalletSystemApi/"]

# Restore NuGet packages
RUN dotnet restore "MiniWalletSystemApi/MiniWalletSystemApi.csproj"

# Copy the remaining source code
COPY MiniWalletSystemApi/ MiniWalletSystemApi/

# Build
WORKDIR "/src/MiniWalletSystemApi"
RUN dotnet build "MiniWalletSystemApi.csproj" -c Release -o /app/build

# 3. Publish Stage
FROM build AS publish
RUN dotnet publish "MiniWalletSystemApi.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# 4. Final Runtime Image
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "MiniWalletSystemApi.dll"]