# 🏪 Microservices E-commerce - Guia de APIs

## 🎯 IMPORTANTE: Use APENAS o API Gateway
**Base URL Principal:** `http://localhost:5000`

---

## 🔐 Autenticação

### Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

---

## 📦 Produtos (via API Gateway)

### 📋 Listar Produtos
```http
GET http://localhost:5000/api/products
Authorization: Bearer {token}
```

### 🔍 Buscar Produto
```http
GET http://localhost:5000/api/products/1
Authorization: Bearer {token}
```

### ➕ Criar Produto
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

### ✏️ Atualizar Produto
```http
PUT http://localhost:5000/api/products/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "id": 1,
  "name": "iPhone 15 Pro Max Atualizado",
  "description": "iPhone 15 Pro Max 512GB",
  "price": 5499.99,
  "stock": 8
}
```

### 🗑️ Deletar Produto
```http
DELETE http://localhost:5000/api/products/1
Authorization: Bearer {token}
```

---

## 🛒 Pedidos (via API Gateway)

### 📋 Listar Pedidos
```http
GET http://localhost:5000/api/orders
Authorization: Bearer {token}
```

### 🔍 Buscar Pedido
```http
GET http://localhost:5000/api/orders/1
Authorization: Bearer {token}
```

### ➕ Criar Pedido
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
    }
  ]
}
```

### 🔄 Atualizar Status
```http
PUT http://localhost:5000/api/orders/1/status
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "Shipped"
}
```

**Status Válidos:** `Pending`, `Processing`, `Shipped`, `Delivered`, `Cancelled`

---

## 🌐 Links de Acesso

### 📊 Swagger Interfaces
- **🚪 API Gateway:** http://localhost:5000/swagger
- **📦 Stock Service:** http://localhost:5001/swagger *(apenas dev)*
- **🛒 Sales Service:** http://localhost:5002/swagger *(apenas dev)*

### 🐰 RabbitMQ Management
- **🌐 URL:** http://localhost:15672
- **👤 Login:** guest / guest

---

## 📊 Credenciais

| Usuário | Senha | Tipo |
|---------|-------|------|
| admin | admin123 | Administrador |
| user | user123 | Usuário |

---

## ⚠️ Regras Importantes

### ✅ Correto
- Use `http://localhost:5000` (API Gateway)
- Inclua `Authorization: Bearer {token}` nos headers
- Use endpoints via Gateway

### ❌ Incorreto
- Acessar `localhost:5001` ou `localhost:5002` diretamente
- Esquecer o token JWT
- Usar URLs dos microserviços em produção

---

## 🚀 Teste Rápido

1. **Login:** POST `/api/auth/login`
2. **Criar Produto:** POST `/api/products` 
3. **Listar Produtos:** GET `/api/products`
4. **Criar Pedido:** POST `/api/orders`
5. **Atualizar Status:** PUT `/api/orders/1/status`

**Todos os endpoints acima usam base: `http://localhost:5000`**
