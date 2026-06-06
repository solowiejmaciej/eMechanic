FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "Services/src/eMechanic.OutboxPublisher/eMechanic.OutboxPublisher.csproj"
RUN dotnet publish "Services/src/eMechanic.OutboxPublisher/eMechanic.OutboxPublisher.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore

FROM --platform=linux/amd64 mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated9.0 AS runtime
WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    FUNCTIONS_WORKER_RUNTIME=dotnet-isolated

