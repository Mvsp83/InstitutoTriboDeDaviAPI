# Receita para construir e rodar a API .NET num container.
# Etapa 1 — "build": usa o SDK do .NET 8 para compilar e publicar.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o código (o .dockerignore mantém bin/obj/.git de fora) e publica só a
# API — que já referencia Application, Domain e Infrastructure. Os projetos
# Web/Tests/FLUTTER não são referenciados pela API, então não entram no build.
COPY . .
RUN dotnet restore InstitutoTriboDeDavi.API/InstitutoTriboDeDavi.API.csproj
RUN dotnet publish InstitutoTriboDeDavi.API/InstitutoTriboDeDavi.API.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# Etapa 2 — "runtime": imagem enxuta, só com o necessário para EXECUTAR.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Escuta na porta que o host injeta (Render usa a variável PORT); sem ela
# (Cloud Run/local) cai em 8080.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet InstitutoTriboDeDavi.API.dll"]
