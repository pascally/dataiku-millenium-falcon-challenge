#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/MilleniumFalcon.CLI/PascalLy.MilleniumFalcon.CLI.csproj", "src/MilleniumFalcon.CLI/"]
COPY ["src/Backend.Application/Backend.Application.csproj", "src/Backend.Application/"]
COPY ["src/Backend.Domain/Backend.Domain.csproj", "src/Backend.Domain/"]
COPY ["src/Backend.Infrastructure/Backend.Infrastructure.csproj", "src/Backend.Infrastructure/"]
RUN dotnet restore "src/MilleniumFalcon.CLI/PascalLy.MilleniumFalcon.CLI.csproj"
COPY . .
WORKDIR "/src/src/MilleniumFalcon.CLI"
RUN dotnet build "PascalLy.MilleniumFalcon.CLI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PascalLy.MilleniumFalcon.CLI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PascalLy.MilleniumFalcon.CLI.dll"]