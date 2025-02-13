#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /URLShortening
COPY ["URLShortening.API/URLShortening.API.csproj", "URLShortening.API/"]
COPY ["URLShortening.Application/URLShortening.Application.csproj", "URLShortening.Application/"]
COPY ["URLShortening.Domain/URLShortening.Domain.csproj", "URLShortening.Domain/"]
COPY ["URLShortening.Infrastructure/URLShortening.Infrastructure.csproj", "URLShortening.Infrastructure/"]
RUN dotnet restore "URLShortening.API/URLShortening.API.csproj"
COPY . .
WORKDIR "/URLShortening/URLShortening.API"
RUN dotnet build "URLShortening.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "URLShortening.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "URLShortening.API.dll"]