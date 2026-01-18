# Dockerfile for .NET 10.0
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore
COPY ["role-play.csproj", "./"]
RUN dotnet restore "role-play.csproj"

# Copy everything and build
COPY . .
RUN dotnet publish "role-play.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "role-play.dll"]