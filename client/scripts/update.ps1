cd ./client/Lykke.Service.MarketProfile.Client/
iwr http://localhost:5000/swagger/v1/swagger.json -o Service.MarketProfile.json
autorest --input-file=Service.MarketProfile.json --csharp --namespace=Lykke.Service.MarketProfile.Client --output-folder=./