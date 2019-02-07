FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM hmbdevopsteam.azurecr.io/webapibackendbase:1 AS build
WORKDIR /src

COPY . Api
WORKDIR /src/Api

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Api.dll"]