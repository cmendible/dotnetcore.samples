FROM microsoft/dotnet:latest
WORKDIR /app
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "debug.on.docker.dll"]
COPY . /app
