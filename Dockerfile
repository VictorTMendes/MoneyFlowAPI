# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o csproj e restaura dependências
COPY MoneyFlowAPI/MoneyFlowAPI.csproj MoneyFlowAPI/
RUN dotnet restore MoneyFlowAPI/MoneyFlowAPI.csproj

# Copia o restante do código e faz o publish
COPY . .
RUN dotnet publish MoneyFlowAPI/MoneyFlowAPI.csproj -c Release -o /app/out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Expõe a porta padrão
EXPOSE 8080

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "MoneyFlowAPI.dll"]
