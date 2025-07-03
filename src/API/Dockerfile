# ------------------------------------------------------------
# 1) Runtime base image
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# ------------------------------------------------------------
# 2) Build stage
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 2.1) Copy shared Contracts for ProjectReference
COPY ["Common/Contracts/Contracts.Shared/Contracts.Shared.csproj", "Common/Contracts/Contracts.Shared/"]

# 2.2) Copy Case-service csproj files
COPY ["Services/Case-Service/src/API/API.csproj",           "Services/Case-Service/src/API/"]
COPY ["Services/Case-Service/src/Application/Application.csproj", "Services/Case-Service/src/Application/"]
COPY ["Services/Case-Service/src/Domain/Domain.csproj",     "Services/Case-Service/src/Domain/"]
COPY ["Services/Case-Service/src/Infrastructure/Infrastructure.csproj", "Services/Case-Service/src/Infrastructure/"]

# 2.3) Restore (will pull in Contracts.Shared)
RUN dotnet restore "Services/Case-Service/src/API/API.csproj"

# 2.4) Copy all source (service + shared)
COPY . .

# 2.5) Publish from the API project folder
WORKDIR "/src/Services/Case-Service/src/API"
RUN dotnet publish "API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# ------------------------------------------------------------
# 3) Final runtime image
# ------------------------------------------------------------
FROM base AS final
WORKDIR /app

# 3.1) Copy and make executable the wait-for-rabbit script
USER root
COPY ["scripts/wait-for-rabbit.sh","/wait-for-rabbit.sh"]
RUN chmod +x /wait-for-rabbit.sh
USER $APP_UID

# 3.2) Copy published output
COPY --from=build /app/publish .

# 3.3) Start only after RabbitMQ is ready
ENTRYPOINT ["/wait-for-rabbit.sh", "rabbitmq", "dotnet", "API.dll"]
