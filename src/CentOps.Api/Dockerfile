FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything
COPY . .

# Restore as distinct layers
RUN dotnet restore ./src/CentOps.Api

# Build and publish a release
RUN dotnet publish -c Release -o out ./src/CentOps.Api

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CentOps.Api.dll"]