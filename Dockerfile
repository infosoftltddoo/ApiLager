# STEP 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Kopiraj sve fajlove u kontejner
COPY . ./

# Restore dependencies (ako ima� vi�e .csproj fajlova, koristi ta�an naziv)
RUN dotnet restore

# Publish aplikaciju u folder /app
RUN dotnet publish -c Release -o /app

# STEP 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Kopiraj build-ovanu aplikaciju iz build stage-a
COPY --from=build /app .

# Defini�i komandu za pokretanje
ENTRYPOINT ["dotnet", "ApiLager.dll"]
