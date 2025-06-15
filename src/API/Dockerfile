# syntax=docker/dockerfile:1

########################
# Stage 0 – base runtime
########################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

########################
# Stage 1 – build
########################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy csproj first for layer caching
COPY ["src/API/API.csproj", "src/API/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]

RUN dotnet restore "src/API/API.csproj"

# copy source and compile
COPY . .
WORKDIR /src/src/API
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

########################
# Stage 2 – publish
########################
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION \
    -o /app/publish /p:UseAppHost=false

########################
# Stage 3 – final image
########################
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "API.dll"]
