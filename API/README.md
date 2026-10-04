# API TechStore Cloud

API REST .NET 8 para cadastro de produtos.

## Rotas

- `GET /api/v1/products`: lista e busca produtos.
- `GET /api/v1/products/{id}`: consulta um produto.
- `POST /api/v1/products`: cadastra um produto.
- `PUT /api/v1/products/{id}`: atualiza um produto.
- `DELETE /api/v1/products/{id}`: exclui um produto.

Swagger: `http://localhost:5093/swagger` · Saúde: `/health/live` e `/health/ready`.

## Executar localmente

Pré-requisitos: .NET 8 ou superior e SQL Server LocalDB (Visual Studio).

```powershell
dotnet restore "TechStore Cloud.sln"
dotnet build "TechStore Cloud.sln"
dotnet test "TechStoreCloud.Tests/TechStoreCloud.Tests.csproj"
dotnet run --project "TechStore Cloud/TechStore Cloud.csproj" --launch-profile http
```

O frontend Angular está em `../FRONT`. Execute `npm install` e `npm start` nesse diretório; a configuração local já aponta para `http://localhost:5093/api/v1`.

