# Setup e Instalação - Microserviços E-commerce

## Pré-requisitos

### Obrigatórios
- .NET 8 SDK
- MySQL (ou usar Docker)
- RabbitMQ (ou usar Docker)

### Opcionais
- Docker Desktop (recomendado)
- Visual Studio 2022 ou VS Code
- Postman ou similar para testes de API

## Setup Rápido com Docker

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd microservicesecommerce
   ```

2. **Inicie as dependências com Docker**
   ```bash
   docker-compose up -d
   ```
   Aguarde alguns segundos para MySQL e RabbitMQ iniciarem.

3. **Execute o script de inicialização**
   ```powershell
   .\scripts\setup-mysql.ps1
   ```

## Setup Manual

### 1. Configurar MySQL

Edite as connection strings nos arquivos `appsettings.json`:
- `src/ApiGateway/appsettings.json`
- `src/StockService/appsettings.json`
- `src/SalesService/appsettings.json`

Exemplo:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Port=3306;Database=NomeDoDB;Uid=developer;Pwd=Luke@2020;"
}
```

### 2. Configurar RabbitMQ

Se não usar Docker, instale RabbitMQ e configure:
```json
"ConnectionStrings": {
  "RabbitMQ": "amqp://usuario:senha@localhost:5672/"
}
```

### 3. Executar Migrações

```bash
# API Gateway
cd src/ApiGateway
dotnet ef database update

# Stock Service  
cd ../StockService
dotnet ef database update

# Sales Service
cd ../SalesService
dotnet ef database update
```

### 4. Executar Serviços

Em terminais separados:

```bash
# Terminal 1 - API Gateway
cd src/ApiGateway
dotnet run

# Terminal 2 - Stock Service
cd src/StockService  
dotnet run

# Terminal 3 - Sales Service
cd src/SalesService
dotnet run
```

## Verificação da Instalação

### 1. Testar APIs

**Registrar usuário:**
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "username": "admin",
  "email": "admin@test.com", 
  "password": "admin123",
  "fullName": "Administrador"
}
```

**Login:**
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Listar produtos (usar token JWT do login):**
```http
GET http://localhost:5000/api/products
Authorization: Bearer {seu_token}
```

### 2. Verificar RabbitMQ

Acesse: http://localhost:15672
- Usuário: guest
- Senha: guest

### 3. Verificar Swagger UIs

- API Gateway: https://localhost:7000/swagger
- Stock Service: https://localhost:7001/swagger  
- Sales Service: https://localhost:7002/swagger

## Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas Stock Service
dotnet test tests/StockService.Tests

# Apenas Sales Service  
dotnet test tests/SalesService.Tests

# Com script PowerShell
.\scripts\run-tests.ps1
```

## Estrutura dos Bancos de Dados

### ApiGatewayDB
- Users (autenticação)

### StockServiceDB  
- Products (produtos e estoque)

### SalesServiceDB
- Orders (pedidos)
- OrderItems (itens dos pedidos)

## Portas dos Serviços

- **API Gateway**: 5000 (HTTP), 7000 (HTTPS)
- **Stock Service**: 5001 (HTTP), 7001 (HTTPS)
- **Sales Service**: 5002 (HTTP), 7002 (HTTPS)
- **MySQL**: 3306
- **RabbitMQ**: 5672 (AMQP), 15672 (Management)

## Solução de Problemas

### Erro de Conexão com MySQL
- Verificar se MySQL está rodando
- Verificar connection string
- Verificar credenciais (developer/Luke@2020)
- Verificar se o banco de dados foi criado

### Erro de Conexão com RabbitMQ
- Verificar se RabbitMQ está rodando
- Verificar porta 5672
- Verificar credenciais (guest/guest)

### Erro de JWT
- Verificar se a chave JWT é a mesma em todos os serviços
- Verificar se o token não expirou (24h)

### Erro de Migração
```bash
# Limpar migrações e recriar
dotnet ef database drop --force
dotnet ef database update
```

## Logs

Cada serviço gera logs em:
- `src/ApiGateway/logs/api-gateway.log`
- `src/StockService/logs/stock-service.log`
- `src/SalesService/logs/sales-service.log`

## Monitoramento

### Eventos RabbitMQ
- `stock-update`: Atualização de estoque
- `order-created`: Pedido criado

### Status HTTP Principais
- 200: Sucesso
- 201: Criado
- 401: Não autorizado
- 404: Não encontrado
- 500: Erro interno
