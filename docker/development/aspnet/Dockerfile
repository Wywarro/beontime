# build .NET app:
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS buildnet

WORKDIR /app

ENV DOTNET_USE_POLLING_FILE_WATCHER 1

COPY *.sln .
COPY BEonTime.Web/Directory.Build.props ./BEonTime.Web/
COPY BEonTime.Web/*.csproj ./BEonTime.Web/
COPY BEonTime.Test/*.csproj ./BEonTime.Test/
COPY BEonTime.Services/*.csproj ./BEonTime.Services/
COPY BEonTime.Data/*.csproj ./BEonTime.Data/

RUN dotnet restore

COPY BEonTime.Web/ ./BEonTime.Web
COPY docker/development/aspnet/docker.launchSettings.json ./BEonTime.Web/Properties/launchSettings.json
COPY BEonTime.Services/ ./BEonTime.Services/
COPY BEonTime.Data/ ./BEonTime.Data/

WORKDIR /app/BEonTime.Web

ENV ASPNETCORE_ENVIRONMENT=DockerDevelopment
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT dotnet watch run --urls=http://+:5000