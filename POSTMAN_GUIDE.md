# 📋 Guia Completo - Endpoints para Postman

## 🚀 Configuração Inicial

### 1. Iniciar os Serviços
```powershell
# Terminal 1 - API Gateway
cd src\ApiGateway
dotnet run

# Terminal 2 - Stock Service  
cd src\StockService
dotnet run

# Terminal 3 - Sales Service
cd src\SalesService
dotnet run
```

### 2. URLs Base dos Serviços
- **API Gateway**: `http://localhost:5000`
- **Stock Service**: `http://localhost:5001` 
- **Sales Service**: `http://localhost:5002`

---

## 🔐 1. AUTENTICAÇÃO (via API Gateway)

### 1.1 Registrar Usuário
**POST** `http://localhost:5000/api/auth/register`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "username": "admin",
  "email": "admin@test.com",
  "password": "admin123",
  "fullName": "Administrador Sistema"
}
```

### 1.2 Login
**POST** `http://localhost:5000/api/auth/login`

**Headers:**
```
Content-Type: application/json
```

**Body (JSON):**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Resposta esperada:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-08-26T15:30:00Z"
}
```

> ⚠️ **IMPORTANTE**: Copie o `token` da resposta para usar nos próximos requests!

---

## 📦 2. PRODUTOS (Stock Service)

### 2.1 Via API Gateway (Recomendado)

#### Listar Todos os Produtos
**GET** `http://localhost:5000/api/products`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

#### Buscar Produto por ID
**GET** `http://localhost:5000/api/products/1`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

#### Criar Produto
**POST** `http://localhost:5000/api/products`

**Headers:**
```
Content-Type: application/json
Authorization: Bearer SEU_TOKEN_AQUI
```

**Body (JSON):**
```json
{
  "name": "Smartphone Samsung Galaxy",
  "description": "Smartphone com tela de 6.1 polegadas",
  "price": 1299.99,
  "stock": 50,
  "category": "Eletrônicos"
}
```

#### Atualizar Produto
**PUT** `http://localhost:5000/api/products/1`

**Headers:**
```
Content-Type: application/json
Authorization: Bearer SEU_TOKEN_AQUI
```

**Body (JSON):**
```json
{
  "id": 1,
  "name": "Smartphone Samsung Galaxy S24",
  "description": "Smartphone premium com tela de 6.1 polegadas",
  "price": 1399.99,
  "stock": 45,
  "category": "Eletrônicos"
}
```

#### Deletar Produto
**DELETE** `http://localhost:5000/api/products/1`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

### 2.2 Direto no Stock Service (Opcional)

#### Listar Produtos (Direto)
**GET** `http://localhost:5001/api/products`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

---

## 🛒 3. PEDIDOS (Sales Service)

### 3.1 Via API Gateway (Recomendado)

#### Listar Todos os Pedidos
**GET** `http://localhost:5000/api/orders`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

#### Buscar Pedido por ID
**GET** `http://localhost:5000/api/orders/1`

**Headers:**
```
Authorization: Bearer SEU_TOKEN_AQUI
```

#### Criar Pedido
**POST** `http://localhost:5000/api/orders`

**Headers:**
```
Content-Type: application/json
Authorization: Bearer SEU_TOKEN_AQUI
```

**Body (JSON):**
```json
{
  "customerId": 1,
  "customerName": "João Silva",
  "customerEmail": "joao@email.com",
  "items": [
    {
      "productId": 1,
      "productName": "Smartphone Samsung Galaxy",
      "quantity": 2,
      "unitPrice": 1299.99
    },
    {
      "productId": 2,
      "productName": "Fone Bluetooth",
      "quantity": 1,
      "unitPrice": 199.99
    }
  ]
}
```

### 3.2 Direto no Sales Service (Opcional)

#### Criar Pedido (Direto)
**POST** `http://localhost:5002/api/orders`

**Headers:**
```
Content-Type: application/json
Authorization: Bearer SEU_TOKEN_AQUI
```

**Body (JSON):**
```json
{
  "customerId": 1,
  "customerName": "Maria Santos",
  "customerEmail": "maria@email.com",
  "items": [
    {
      "productId": 1,
      "productName": "Notebook Dell",
      "quantity": 1,
      "unitPrice": 2999.99
    }
  ]
}
```

---

## 🧪 4. SEQUÊNCIA DE TESTES RECOMENDADA

### Passo 1: Autenticação
1. Registrar um usuário
2. Fazer login e guardar o token

### Passo 2: Criar Produtos
1. Criar 3-4 produtos diferentes
2. Listar produtos para verificar

### Passo 3: Fazer Pedidos
1. Criar um pedido com produtos existentes
2. Verificar se o estoque foi atualizado
3. Listar pedidos

### Passo 4: Validar Integração
1. Criar mais produtos
2. Fazer vários pedidos
3. Verificar logs dos serviços

---

## 📊 5. EXEMPLOS DE DADOS PARA TESTES

### Produtos para Criar:
```json
{
  "name": "Notebook Dell Inspiron",
  "description": "Notebook para trabalho e estudos",
  "price": 2999.99,
  "stock": 25,
  "category": "Informática"
}
```

```json
{
  "name": "Mouse Gamer RGB",
  "description": "Mouse com iluminação RGB",
  "price": 89.99,
  "stock": 100,
  "category": "Periféricos"
}
```

```json
{
  "name": "Teclado Mecânico",
  "description": "Teclado mecânico para gaming",
  "price": 299.99,
  "stock": 75,
  "category": "Periféricos"
}
```

### Pedidos para Criar:
```json
{
  "customerId": 1,
  "customerName": "Ana Costa",
  "customerEmail": "ana@email.com",
  "items": [
    {
      "productId": 1,
      "productName": "Notebook Dell Inspiron",
      "quantity": 1,
      "unitPrice": 2999.99
    },
    {
      "productId": 2,
      "productName": "Mouse Gamer RGB",
      "quantity": 1,
      "unitPrice": 89.99
    }
  ]
}
```

---

## ⚡ 6. DICAS PARA POSTMAN

### Configurar Environment (Opcional)
1. Crie um Environment no Postman
2. Adicione variáveis:
   - `gateway_url`: `http://localhost:5000`
   - `stock_url`: `http://localhost:5001`
   - `sales_url`: `http://localhost:5002`
   - `token`: (será preenchido após login)

### Scripts para Automatizar Token
No teste de **Login**, aba **Tests**, adicione:
```javascript
if (responseCode.code === 200) {
    var jsonData = pm.response.json();
    pm.environment.set("token", jsonData.token);
}
```

### Headers Dinâmicos
Use `{{token}}` nos headers Authorization:
```
Authorization: Bearer {{token}}
```

---

## 🔍 7. VERIFICAÇÃO DE FUNCIONAMENTO

### Logs para Acompanhar
- **API Gateway**: `src/ApiGateway/logs/api-gateway.log`
- **Stock Service**: `src/StockService/logs/stock-service.log`
- **Sales Service**: `src/SalesService/logs/sales-service.log`

### Status HTTP Esperados
- **200**: Sucesso (GET, PUT)
- **201**: Criado (POST)
- **204**: Sem conteúdo (DELETE)
- **401**: Não autorizado (sem token ou token inválido)
- **404**: Não encontrado
- **400**: Dados inválidos

---

## 🎯 8. TROUBLESHOOTING

### Erro 401 (Unauthorized)
- Verificar se o token está correto
- Verificar se o token não expirou (24h)
- Verificar se está usando `Bearer TOKEN`

### Erro 404 (Not Found)
- Verificar se os serviços estão rodando
- Verificar se as URLs estão corretas
- Verificar se o produto/pedido existe

### Erro 500 (Internal Server Error)
- Verificar logs dos serviços
- Verificar se MySQL está rodando
- Verificar se RabbitMQ está rodando

### Portas dos Serviços
- **API Gateway**: 5000 (HTTP), 7000 (HTTPS)
- **Stock Service**: 5001 (HTTP), 7001 (HTTPS)
- **Sales Service**: 5002 (HTTP), 7002 (HTTPS)
