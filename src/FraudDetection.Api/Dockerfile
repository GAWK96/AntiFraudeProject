FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/FraudDetection.Api/FraudDetection.Api.csproj", "src/FraudDetection.Api/"]
COPY ["src/FraudDetection.Application/FraudDetection.Application.csproj", "src/FraudDetection.Application/"]
COPY ["src/FraudDetection.Domain/FraudDetection.Domain.csproj", "src/FraudDetection.Domain/"]
COPY ["src/FraudDetection.Infrastructure/FraudDetection.Infrastructure.csproj", "src/FraudDetection.Infrastructure/"]

RUN dotnet restore "src/FraudDetection.Api/FraudDetection.Api.csproj"

COPY . .

RUN dotnet publish "src/FraudDetection.Api/FraudDetection.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FraudDetection.Api.dll"]