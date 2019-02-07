FROM microsoft/dotnet:2.2-sdk
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY **/*.csproj Api/
WORKDIR /src/Api

RUN dotnet restore