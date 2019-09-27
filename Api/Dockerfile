FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk as build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj Api/
WORKDIR /src/Api

RUN dotnet restore

WORKDIR /src

COPY . Api
WORKDIR /src/Api

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Api.dll"]
