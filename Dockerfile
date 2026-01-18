# Dockerfile for .NET 10.0 - FIXED VERSION
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only project file first for better caching
COPY ["role-play.csproj", "./"]
RUN dotnet restore

# Copy everything else
COPY . .
RUN dotnet publish -c Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "role-play.dll"]