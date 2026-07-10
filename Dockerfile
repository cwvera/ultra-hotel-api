FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore — csproj files primero para cache de capas
COPY ultra.hotel/UltraHotel.Commons/UltraHotel.Commons.csproj             UltraHotel.Commons/
COPY ultra.hotel/UltraHotel.Domain/UltraHotel.Domain.csproj               UltraHotel.Domain/
COPY ultra.hotel/UltraHotel.Application/UltraHotel.Application.csproj     UltraHotel.Application/
COPY ultra.hotel/UltraHotel.Infrastructure/UltraHotel.Infrastructure.csproj UltraHotel.Infrastructure/
COPY ultra.hotel/UltraHotel.WebApi/UltraHotel.WebApi.csproj               UltraHotel.WebApi/

RUN dotnet restore UltraHotel.WebApi/UltraHotel.WebApi.csproj

# Build
COPY ultra.hotel/UltraHotel.Commons/        UltraHotel.Commons/
COPY ultra.hotel/UltraHotel.Domain/         UltraHotel.Domain/
COPY ultra.hotel/UltraHotel.Application/    UltraHotel.Application/
COPY ultra.hotel/UltraHotel.Infrastructure/ UltraHotel.Infrastructure/
COPY ultra.hotel/UltraHotel.WebApi/         UltraHotel.WebApi/

RUN dotnet publish UltraHotel.WebApi/UltraHotel.WebApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "UltraHotel.WebApi.dll"]
