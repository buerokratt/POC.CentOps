FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . .

# Restore as distinct layers
RUN dotnet restore CentOps.Api.csproj

# Build and publish a release
RUN dotnet publish -c Release -o out CentOps.Api.csproj

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CentOps.Api.dll"]
