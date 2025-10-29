# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas o csproj e restaura as dependências
COPY MoneyFlowAPI/MoneyFlowAPI.csproj MoneyFlowAPI/
RUN dotnet restore "MoneyFlowAPI/MoneyFlowAPI.csproj"

# Copia todo o código e compila
COPY . .
WORKDIR /src/MoneyFlowAPI
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Render usa a variável PORT
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "MoneyFlowAPI.dll"]
