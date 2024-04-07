# Utiliza la imagen de ASP.NET Core como base
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos de la aplicación y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia el resto de los archivos y realiza la compilación
COPY . ./
RUN dotnet publish -c Release -o out

# Configura el contenedor y establece el punto de entrada
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "MiAPI.dll"]
