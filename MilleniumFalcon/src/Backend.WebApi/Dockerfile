#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Backend.WebApi/Backend.WebApi.csproj", "src/Backend.WebApi/"]
COPY ["src/Backend.Application/Backend.Application.csproj", "src/Backend.Application/"]
COPY ["src/Backend.Domain/Backend.Domain.csproj", "src/Backend.Domain/"]
COPY ["src/Backend.Infrastructure/Backend.Infrastructure.csproj", "src/Backend.Infrastructure/"]
RUN dotnet restore "src/Backend.WebApi/Backend.WebApi.csproj"
COPY . .
WORKDIR "/src/src/Backend.WebApi"
RUN dotnet build "Backend.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add `www-data` to group `appuser`
RUN addgroup --gid 1000 appuser; \
  adduser --uid 1000 --gid 1000 --disabled-password appuser; \
  adduser www-data appuser;

ENTRYPOINT ["dotnet", "Backend.WebApi.dll"]