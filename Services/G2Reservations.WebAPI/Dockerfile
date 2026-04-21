FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/G2Reservations.WebAPI/G2Reservations.WebAPI.csproj", "Services/G2Reservations.WebAPI/"]
COPY ["Shared/G2_Shared_Infrastructure/G2_Shared_Infrastructure.csproj", "Shared/G2_Shared_Infrastructure/"]
RUN dotnet restore "./Services/G2Reservations.WebAPI/G2Reservations.WebAPI.csproj"
COPY . .
WORKDIR "/src/Services/G2Reservations.WebAPI"
RUN dotnet build "./G2Reservations.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./G2Reservations.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "G2Reservations.WebAPI.dll"]