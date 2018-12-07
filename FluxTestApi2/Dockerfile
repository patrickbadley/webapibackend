FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 50407
EXPOSE 44386

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["FluxTestApi2.csproj", "FluxTestApi2/"]
COPY NuGet.Config /root/.nuget/NuGet/
RUN dotnet restore "FluxTestApi2/FluxTestApi2.csproj"
COPY . FluxTestApi2
WORKDIR "/src/FluxTestApi2"
RUN dotnet build "FluxTestApi2.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FluxTestApi2.csproj" -c Release -o /app


# Use opnssl to generate a self signed certificate cert.pfx with password $env:certPassword
WORKDIR /app
ENV certPassword 1234
RUN openssl genrsa -des3 -passout pass:${certPassword} -out server.key 2048
RUN openssl rsa -passin pass:${certPassword} -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=bmw-ref-arch.eastus.cloudapp.azure.com'
RUN openssl x509 -req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out cert.pfx -inkey server.key -in server.crt -certfile server.crt -passout pass:${certPassword}

# Expose port 443 for the application.
EXPOSE 443
FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "FluxTestApi2.dll"]