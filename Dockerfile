# 1. Stage: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopíruj projektové soubory
COPY . ./

# Publikuj aplikaci
RUN dotnet publish -c Release -o /out

# 2. Stage: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Zkopíruj publikované soubory
COPY --from=build /out .

# Exponuj port 80
EXPOSE 80

# Spusť aplikaci
ENTRYPOINT ["dotnet", "WebApp.dll"]
