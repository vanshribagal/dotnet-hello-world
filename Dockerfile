# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY hello-world-api/hello-world-api.csproj ./hello-world-api/
RUN dotnet restore hello-world-api/hello-world-api.csproj

# Copy remaining source code
COPY . .
WORKDIR /app/hello-world-api

# Build and publish
RUN dotnet publish -c Release -o /out

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY --from=build /out .
EXPOSE 80

ENTRYPOINT ["dotnet", "hello-world-api.dll"]
