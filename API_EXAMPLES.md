# Exemplos de uso da API - Microserviços E-commerce

## 1. Autenticação

### Registrar usuário
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "username": "usuario_teste",
  "email": "usuario@teste.com",
  "password": "senha123",
  "fullName": "Usuário de Teste"
}
```

### Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "username": "usuario_teste",
  "password": "senha123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "message": "Login realizado com sucesso"
}
```

## 2. Gestão de Produtos (via API Gateway)

### Listar produtos
```http
GET http://localhost:5000/api/products
Authorization: Bearer {seu_token_jwt}
```

### Buscar produto específico
```http
GET http://localhost:5000/api/products/1
Authorization: Bearer {seu_token_jwt}
```

### Criar novo produto
```http
POST http://localhost:5000/api/products
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json

{
  "name": "Smartphone Samsung",
  "description": "Smartphone Samsung Galaxy com 128GB",
  "price": 1200.00,
  "stockQuantity": 50
}
```

### Atualizar produto
```http
PUT http://localhost:5000/api/products/1
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json

{
  "name": "Notebook Dell Atualizado",
  "description": "Notebook Dell Inspiron 15 - Versão Atualizada",
  "price": 2800.00,
  "stockQuantity": 8
}
```

### Verificar disponibilidade de estoque
```http
POST http://localhost:5000/api/products/1/check-stock
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json

5
```

## 3. Gestão de Pedidos (via API Gateway)

### Listar pedidos do usuário
```http
GET http://localhost:5000/api/orders
Authorization: Bearer {seu_token_jwt}
```

### Buscar pedido específico
```http
GET http://localhost:5000/api/orders/1
Authorization: Bearer {seu_token_jwt}
```

### Criar novo pedido
```http
POST http://localhost:5000/api/orders
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json

{
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

### Atualizar status do pedido
```http
PUT http://localhost:5000/api/orders/1/status
Authorization: Bearer {seu_token_jwt}
Content-Type: application/json

3
```

## 4. Acesso direto aos microserviços (para desenvolvimento)

### Stock Service (Porta 7001)
```http
GET https://localhost:7001/api/products
Authorization: Bearer {seu_token_jwt}
```

### Sales Service (Porta 7002)
```http
GET https://localhost:7002/api/orders
Authorization: Bearer {seu_token_jwt}
```

## 5. Status de pedidos

- 1: Pending (Pendente)
- 2: Confirmed (Confirmado)
- 3: Shipped (Enviado)
- 4: Delivered (Entregue)
- 5: Cancelled (Cancelado)

## 6. Fluxo completo de uma venda

1. **Registrar/Login** para obter token JWT
2. **Listar produtos** disponíveis
3. **Verificar estoque** dos produtos desejados
4. **Criar pedido** com os produtos
5. **Acompanhar status** do pedido

## 7. Monitoramento

### RabbitMQ Management
- URL: http://localhost:15672
- Usuário: guest
- Senha: guest

### Swagger UIs
- API Gateway: https://localhost:7000/swagger
- Stock Service: https://localhost:7001/swagger
- Sales Service: https://localhost:7002/swagger

## 8. Logs

Os logs são salvos na pasta `logs/` de cada serviço:
- `logs/api-gateway.log`
- `logs/stock-service.log`
- `logs/sales-service.log`
