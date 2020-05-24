# https://docs.docker.com/engine/examples/dotnetcore/
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish
WORKDIR /app

# Install Node JS 10
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_10.x | bash \
    && apt-get install nodejs -yq

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish --no-restore -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aggregail.MongoDB.Admin.dll"]