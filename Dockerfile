# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia a solution e todos os projetos
COPY InstitutoTriboDaviAPI.sln ./
COPY InstitutoTriboDeDavi.API/InstitutoTriboDeDavi.API.csproj InstitutoTriboDeDavi.API/
COPY InstitutoTriboDeDavi.Common.BaseEntity/InstitutoTriboDeDavi.Common.BaseEntity.csproj InstitutoTriboDeDavi.Common.BaseEntity/
COPY InstitutoTriboDeDavi.Common.DataAccess/InstitutoTriboDeDavi.Common.DataAccess.csproj InstitutoTriboDeDavi.Common.DataAccess/
COPY InstitutoTriboDeDavi.System.Core/InstitutoTriboDeDavi.System.Core.csproj InstitutoTriboDeDavi.System.Core/
COPY InstitutoTriboDeDavi.System.DataAccess/InstitutoTriboDeDavi.System.DataAccess.csproj InstitutoTriboDeDavi.System.DataAccess/
COPY InstitutoTriboDeDavi.System.DataAccess.Business/InstitutoTriboDeDavi.System.DataAccess.Business.csproj InstitutoTriboDeDavi.System.DataAccess.Business/
COPY InstitutoTriboDeDavi.System.Domain/InstitutoTriboDeDavi.System.Domain.csproj InstitutoTriboDeDavi.System.Domain/
COPY InstitutoTriboDeDavi.System.DTO/InstitutoTriboDeDavi.System.DTO.csproj InstitutoTriboDeDavi.System.DTO/
COPY InstitutoTriboDeDavi.System.Factory/InstitutoTriboDeDavi.System.Factory.csproj InstitutoTriboDeDavi.System.Factory/
COPY InstitutoTriboDeDavi.System.Infra/InstitutoTriboDeDavi.System.Infra.csproj InstitutoTriboDeDavi.System.Infra/
COPY InstitutoTriboDeDavi.System.Services/InstitutoTriboDeDavi.System.Services.csproj InstitutoTriboDeDavi.System.Services/
COPY InstitutoTriboDeDavi.System.Services.Business/InstitutoTriboDeDavi.System.Services.Business.csproj InstitutoTriboDeDavi.System.Services.Business/

# Restaura os pacotes NuGet
RUN dotnet restore InstitutoTriboDaviAPI.sln

# Copia todo o código fonte
COPY . .

# Publica a API em modo Release
RUN dotnet publish InstitutoTriboDeDavi.API/InstitutoTriboDeDavi.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Estágio 2: Runtime (imagem final menor)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Porta que o Render vai usar
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "InstitutoTriboDeDavi.API.dll"]
