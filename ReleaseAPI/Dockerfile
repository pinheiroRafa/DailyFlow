# Usar imagem base do .NET SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar arquivos da API para dentro do contêiner
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Usar uma imagem mais leve do .NET para rodar a API
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expor a porta da API (alterar se necessário)
EXPOSE 8082

# Comando para rodar a API
CMD ["dotnet", "ReleaseAPI.dll", "http://0.0.0.0:8082"]