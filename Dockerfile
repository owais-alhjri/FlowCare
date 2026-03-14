FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["*.sln", "./"]
COPY ["src/FlowCare.API/FlowCare.API.csproj", "src/FlowCare.API/"]
COPY ["src/FlowCare.Application/FlowCare.Application.csproj", "src/FlowCare.Application/"]
COPY ["src/FlowCare.Domain/FlowCare.Domain.csproj", "src/FlowCare.Domain/"]
COPY ["src/FlowCare.Infrastructure/FlowCare.Infrastructure.csproj", "src/FlowCare.Infrastructure/"]
RUN dotnet restore "src/FlowCare.API/FlowCare.API.csproj"
COPY . .
RUN dotnet build "src/FlowCare.API/FlowCare.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "src/FlowCare.API/FlowCare.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlowCare.API.dll"]