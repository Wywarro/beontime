# build .NET app:
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /src

COPY beontime.backend/*.sln .
COPY beontime.backend/src/Beontime.WebApi/*.csproj ./Beontime.WebApi/
COPY beontime.backend/src/Beontime.Infrastructure/*.csproj ./Beontime.Infrastructure/
COPY beontime.backend/src/Beontime.Application/*.csproj ./Beontime.Application/
COPY beontime.backend/src/Beontime.Domain/*.csproj ./Beontime.Domain/

RUN dotnet restore ./Beontime.WebApi/Beontime.WebApi.csproj

COPY beontime.backend/src/Beontime.WebApi ./Beontime.WebApi/
COPY beontime.backend/src/Beontime.Infrastructure ./Beontime.Infrastructure/
COPY beontime.backend/src/Beontime.Application ./Beontime.Application/
COPY beontime.backend/src/Beontime.Domain ./Beontime.Domain/

RUN dotnet publish ./Beontime.WebApi/Beontime.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

COPY --from=build /app/publish .

RUN groupadd -g 10000 dotnet && useradd -u 10000 -g dotnet dotnet
RUN chown -R dotnet:dotnet /app
USER dotnet:dotnet

ENTRYPOINT ["dotnet", "Beontime.WebApi.dll"]