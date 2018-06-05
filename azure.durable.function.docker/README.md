# Run a .NET Core Azure Durable Function in a Container

## Build the dotnet core project

``` powershell
dotnet build -o .\wwwroot\bin\
```

## Build the docker image

``` powershell
docker build --build-arg STORAGE_ACCOUNT='[Storage Account Connection String]' -t durable .
```

## Run the Azure Durable Functions in a container

``` powershell
docker run -it -p 5000:80 durable
```

## Start the Azure Durable Function

``` bash
curl -X POST http://localhost:5000/api/orchestrators/orchestrator -i -H "Content-Length: 0"
```

## To Debug on localhost

``` powershell
npm install -g azure-functions-core-tools@core
```

``` powershell
cd wwwroot
func start --debug VSCode
```