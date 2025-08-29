# 🏪 Microservices E-commerce - Documentação Completa das APIs

## 📋 Índice
- [🔐 Autenticação](#-autenticação)
- [🚪 API Gateway](#-api-gateway)
- [📦 Stock Service](#-stock-service)
- [🛒 Sales Service](#-sales-service)
- [🐰 RabbitMQ Management](#-rabbitmq-management)
- [📊 Status dos Serviços](#-status-dos-serviços)

---

## 🔐 Autenticação

### 🔑 Credenciais Disponíveis
| Usuário | Senha | Perfil |
|---------|-------|--------|
| `admin` | `admin123` | Administrador |
| `user` | `user123` | Usuário |

---

## 🚪 API Gateway
**Base URL:** `http://localhost:5000`
**Swagger:** `http://localhost:5000/swagger`

### 🔐 Authentication Endpoints

#### ✅ Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@test.com",
    "fullName": "Administrador"
  }
}
```

### 📦 Products Endpoints (via Gateway)

#### 📋 Listar Todos os Produtos
```http
GET http://localhost:5000/api/products
Authorization: Bearer {token}
```

#### 🔍 Buscar Produto por ID
```http
GET http://localhost:5000/api/products/{id}
Authorization: Bearer {token}
```

#### ➕ Criar Produto
```http
POST http://localhost:5000/api/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Smartphone Samsung Galaxy S24",
  "description": "Galaxy S24 Ultra com 256GB",
  "price": 2999.99,
  "stock": 50
}
```

#### ✏️ Atualizar Produto
```http
PUT http://localhost:5000/api/products/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Smartphone Samsung Galaxy S24 Ultra",
  "description": "Galaxy S24 Ultra com 512GB - Atualizado",
  "price": 3299.99,
  "stock": 30
}
```

#### 🗑️ Deletar Produto
```http
DELETE http://localhost:5000/api/products/{id}
Authorization: Bearer {token}
```

### 🛒 Orders Endpoints (via Gateway)

#### 📋 Listar Todos os Pedidos
```http
GET http://localhost:5000/api/orders
Authorization: Bearer {token}
```

#### 🔍 Buscar Pedido por ID
```http
GET http://localhost:5000/api/orders/{id}
Authorization: Bearer {token}
```

#### ➕ Criar Pedido
```http
POST http://localhost:5000/api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": "customer001",
  "customerName": "João Silva",
  "customerEmail": "joao@email.com",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

#### 🔄 Atualizar Status do Pedido
```http
PUT http://localhost:5000/api/orders/{id}/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Shipped"
}
```

**Status Válidos:** `Pending`, `Processing`, `Shipped`, `Delivered`, `Cancelled`

---

## 📦 Stock Service
**Base URL:** `http://localhost:5001` *(Apenas para desenvolvimento - Use Gateway em produção)*
**Swagger:** `http://localhost:5001/swagger`

### 📊 Products Management

#### 📋 GET /api/products
```http
GET http://localhost:5001/api/products
Authorization: Bearer {token}
```

#### 🔍 GET /api/products/{id}
```http
GET http://localhost:5001/api/products/1
Authorization: Bearer {token}
```

#### ➕ POST /api/products
```http
POST http://localhost:5001/api/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Notebook Dell Inspiron 15",
  "description": "Notebook para trabalho com SSD 512GB",
  "price": 3499.99,
  "stock": 25
}
```

#### ✏️ PUT /api/products/{id}
```http
PUT http://localhost:5001/api/products/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "Notebook Dell Inspiron 15 Pro",
  "description": "Notebook profissional com SSD 1TB",
  "price": 4299.99,
  "stock": 20
}
```

#### 🗑️ DELETE /api/products/{id}
```http
DELETE http://localhost:5001/api/products/1
Authorization: Bearer {token}
```

### 🔄 Internal Endpoints (Service-to-Service)

#### 🔍 GET /api/products/internal/{id}
```http
GET http://localhost:5001/api/products/internal/1
Authorization: Bearer {token}
```

#### 📉 PUT /api/products/internal/{id}/reduce-stock
```http
PUT http://localhost:5001/api/products/internal/1/reduce-stock
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 5
}
```

---

## 🛒 Sales Service
**Base URL:** `http://localhost:5002` *(Apenas para desenvolvimento - Use Gateway em produção)*
**Swagger:** `http://localhost:5002/swagger`

### 📊 Orders Management

#### 📋 GET /api/orders
```http
GET http://localhost:5002/api/orders
Authorization: Bearer {token}
```

#### 🔍 GET /api/orders/{id}
```http
GET http://localhost:5002/api/orders/1
Authorization: Bearer {token}
```

#### ➕ POST /api/orders
```http
POST http://localhost:5002/api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": "customer002",
  "customerName": "Maria Santos",
  "customerEmail": "maria@email.com",
  "items": [
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

#### 🔄 PUT /api/orders/{id}/status
```http
PUT http://localhost:5002/api/orders/1/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Processing"
}
```

### 🔄 Internal Endpoints (Service-to-Service)

#### 🔍 GET /api/orders/internal/{id}
```http
GET http://localhost:5002/api/orders/internal/1
Authorization: Bearer {token}
```

---

## 🐰 RabbitMQ Management
**URL:** `http://localhost:15672`
**Credenciais:** `guest` / `guest`

### 📊 Funcionalidades Disponíveis
- **Queues:** Visualizar filas de mensagens
- **Exchanges:** Gerenciar roteamento de mensagens
- **Connections:** Monitorar conexões ativas
- **Channels:** Visualizar canais de comunicação

### 🔄 Filas Utilizadas
- `stock.product.created` - Notificações de produtos criados
- `stock.product.updated` - Notificações de produtos atualizados
- `sales.order.created` - Notificações de pedidos criados
- `sales.order.status.changed` - Mudanças de status de pedidos

---

## 📊 Status dos Serviços

### ✅ Verificação de Saúde
```bash
# API Gateway
curl http://localhost:5000/swagger

# Stock Service
curl http://localhost:5001/swagger

# Sales Service
curl http://localhost:5002/swagger

# RabbitMQ
curl http://localhost:15672
```

### 🔧 Comandos de Inicialização
```bash
# RabbitMQ (Docker)
docker start rabbitmq

# API Gateway
cd "src/ApiGateway" && dotnet run

# Stock Service
cd "src/StockService" && dotnet run

# Sales Service
cd "src/SalesService" && dotnet run
```

---

## 📝 Exemplos de Uso Completo

### 🎯 Fluxo de Teste Recomendado

#### 1️⃣ Autenticação
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

#### 2️⃣ Criar Produto
```http
POST http://localhost:5000/api/products
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "iPhone 15 Pro Max",
  "description": "iPhone 15 Pro Max 256GB",
  "price": 4999.99,
  "stock": 10
}
```

#### 3️⃣ Listar Produtos
```http
GET http://localhost:5000/api/products
Authorization: Bearer {token}
```

#### 4️⃣ Criar Pedido
```http
POST http://localhost:5000/api/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerId": "customer003",
  "customerName": "Pedro Costa",
  "customerEmail": "pedro@email.com",
  "items": [
    {
      "productId": 1,
      "quantity": 1
    }
  ]
}
```

#### 5️⃣ Atualizar Status
```http
PUT http://localhost:5000/api/orders/1/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Shipped"
}
```

---

## ⚠️ Regras Importantes

### 🎯 Para Produção
- ✅ **USE APENAS:** `http://localhost:5000` (API Gateway)
- ❌ **NUNCA ACESSE:** `localhost:5001` ou `localhost:5002` diretamente

### 🔐 Segurança
- Todos os endpoints (exceto login) requerem JWT
- Token expira em 24 horas
- Use HTTPS em produção

### 📊 Monitoramento
- RabbitMQ Management: `http://localhost:15672`
- Logs disponíveis no console de cada serviço
- Swagger disponível em todos os serviços

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Verificar logs dos serviços
2. Confirmar status no RabbitMQ Management
3. Testar endpoints via Swagger
4. Verificar autenticação JWT

---

*Documentação atualizada em: ${new Date().toLocaleDateString('pt-BR')}*
