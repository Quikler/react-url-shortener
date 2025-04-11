WebApi:
	dotnet run --project ./react-url-shortener.Server/src/WebApi/WebApi.csproj

Client:
	cd ./react-url-shortener.Client/ && npm run dev

start:
	make WebApi & make Client
