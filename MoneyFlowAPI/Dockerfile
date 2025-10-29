# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos do projeto e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante e publica em modo Release
COPY . .
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime (executar o app)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Expõe a porta padrão usada pelo .NET
EXPOSE 8080

# Define o comando de inicialização
ENTRYPOINT ["dotnet", "MoneyFlowAPI.dll"]
