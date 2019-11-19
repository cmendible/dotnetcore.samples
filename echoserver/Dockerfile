FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS builder
COPY . src
WORKDIR src
RUN dotnet restore
RUN dotnet publish -c release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
COPY --from=builder src/bin/release/netcoreapp3.0/publish app
WORKDIR app
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80
ENTRYPOINT ["dotnet", "echoserver.dll"]