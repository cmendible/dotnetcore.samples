## RUN:

dapr run --app-id routing --app-port 5000 dotnet run

## REST:

curl -X POST http://127.0.0.1:5000/deposit -H "Content-Type: application/json" -d '{ \"id\": \"17\", \"amount\": 12 }'

curl -X POST http://127.0.0.1:5000/withdraw -H "Content-Type: application/json" -d '{ \"id\": \"17\", \"amount\": 10 }'

curl -X GET http://127.0.0.1:5000/17 -H "Content-Type: application/json"

## PUB/SUB

dapr publish -t withdraw -p '{\"id\": \"17\", \"amount\": 15 }'
dapr publish -t deposit -p '{\"id\": \"17\", \"amount\": 15 }'

curl -X GET http://127.0.0.1:5000/17 -H "Content-Type: application/json"