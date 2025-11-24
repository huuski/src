# Use a imagem base do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["PresentationLayer/PresentationLayer.csproj", "PresentationLayer/"]
COPY ["ApplicationLayer/ApplicationLayer.csproj", "ApplicationLayer/"]
COPY ["DomainLayer/DomainLayer.csproj", "DomainLayer/"]
COPY ["InfrastructureLayer/InfrastructureLayer.csproj", "InfrastructureLayer/"]

# Restaurar dependências
RUN dotnet restore "PresentationLayer/PresentationLayer.csproj"

# Copiar todo o código fonte
COPY . .

# Build e publicar
WORKDIR "/src/PresentationLayer"
RUN dotnet build "PresentationLayer.csproj" -c Release -o /app/build
RUN dotnet publish "PresentationLayer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagem final runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copiar arquivos publicados
COPY --from=build /app/publish .

# Expor porta (será sobrescrita pela variável PORT se necessário)
EXPOSE 8080

# Variável de ambiente para porta
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Comando de inicialização
ENTRYPOINT ["dotnet", "PresentationLayer.dll"]

