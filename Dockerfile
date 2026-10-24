FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY IpDeputyApi.csproj ./
RUN dotnet restore IpDeputyApi.csproj

COPY . .
RUN dotnet publish IpDeputyApi.csproj -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_NOLOGO=true

RUN mkdir -p /app/Logs && chown -R $APP_UID /app

COPY --from=build --chown=$APP_UID /app/publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "IpDeputyApi.dll"]
