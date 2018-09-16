FROM microsoft/dotnet:2.0.6-sdk-2.1.101-jessie AS builder
COPY ./*.csproj ./src/
WORKDIR /src
RUN dotnet restore
COPY . /src
RUN dotnet publish -c release

FROM microsoft/dotnet:2.0.6-runtime-jessie
COPY --from=builder src/bin/release/netcoreapp2.0/publish app
WORKDIR app
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80
ENTRYPOINT ["dotnet", "echoserver.dll"]