# Etapa 1: compilar con el SDK completo
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo el .csproj: así Docker reutiliza los paquetes si no cambiaron
COPY InventarioIT.Api/InventarioIT.Api.csproj InventarioIT.Api/
RUN dotnet restore InventarioIT.Api/InventarioIT.Api.csproj

COPY InventarioIT.Api/ InventarioIT.Api/
RUN dotnet publish InventarioIT.Api/InventarioIT.Api.csproj -c Release -o /app/publish --no-restore

# Etapa 2: imagen final, solo con lo necesario para ejecutar (mucho más liviana)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Ejecutar sin permisos de administrador
USER $APP_UID

ENTRYPOINT ["dotnet", "InventarioIT.Api.dll"]
