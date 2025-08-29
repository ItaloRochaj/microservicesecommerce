# 🚀 Guia Rápido - Como Usar no Postman

## 📥 Importar Collection e Environment

### 1. Importar Collection
1. Abra o Postman
2. Clique em **Import** (canto superior esquerdo)
3. Arraste o arquivo `Postman_Collection.json` ou clique em **Upload Files**
4. Clique em **Import**

### 2. Importar Environment  
1. No Postman, clique na engrenagem ⚙️ (canto superior direito)
2. Clique em **Import**
3. Arraste o arquivo `Postman_Environment.json` ou clique em **Upload Files**
4. Clique em **Import**
5. Selecione o environment **"Microservices E-commerce Environment"**

## ⚡ Sequência de Testes Rápida

### Passo 1: Iniciar Serviços
```powershell
# Terminal 1 - API Gateway
cd src\ApiGateway && dotnet run

# Terminal 2 - Stock Service  
cl

# Terminal 3 - Sales Service
cd src\SalesService && dotnet run
```

### Passo 2: Autenticação
1. **Register User** - Para criar um usuário
2. **Login** - O token será salvo automaticamente no environment

### Passo 3: Criar Produtos
1. **Create Product - Smartphone**
2. **Create Product - Notebook** 
3. **Create Product - Mouse Gamer**
4. **List All Products** - Para verificar

### Passo 4: Fazer Pedidos
1. **Create Order - Mixed Products** - Pedido com múltiplos produtos
2. **Create Order - Single Product** - Pedido com um produto
3. **List All Orders** - Para verificar

### Passo 5: Validar Funcionamento
1. **List All Products** - Verificar se estoque foi atualizado
2. **Get Order by ID** - Verificar detalhes do pedido

## 📋 Endpoints Disponíveis na Collection

### 🔐 Authentication
- **Register User** - Criar usuário
- **Login** - Fazer login e salvar token

### 📦 Products (via Gateway) - RECOMENDADO
- **List All Products** - GET produtos
- **Get Product by ID** - GET produto específico
- **Create Product - Smartphone** - POST smartphone
- **Create Product - Notebook** - POST notebook  
- **Create Product - Mouse Gamer** - POST mouse
- **Update Product** - PUT atualizar produto
- **Delete Product** - DELETE remover produto

### 🛒 Orders (via Gateway) - RECOMENDADO
- **List All Orders** - GET pedidos
- **Get Order by ID** - GET pedido específico
- **Create Order - Mixed Products** - POST pedido múltiplos itens
- **Create Order - Single Product** - POST pedido item único

### 📦 Products (Direct Stock Service) - OPCIONAL
- **List Products (Direct)** - Acesso direto ao Stock Service
- **Create Product (Direct)** - Criar produto direto

### 🛒 Orders (Direct Sales Service) - OPCIONAL  
- **List Orders (Direct)** - Acesso direto ao Sales Service
- **Create Order (Direct)** - Criar pedido direto

## 💡 Dicas Importantes

### ✅ Use Sempre o API Gateway
- Prefira os endpoints **"via Gateway"** 
- Eles passam pela autenticação e roteamento corretos

### 🔑 Token Automático
- O login salva o token automaticamente
- Não precisa copiar/colar manualmente

### 📊 Monitoramento
- Acompanhe os logs nos terminais dos serviços
- Verifique se o RabbitMQ está processando mensagens

### 🐛 Troubleshooting
- **401 Unauthorized**: Faça login novamente
- **404 Not Found**: Verifique se o serviço está rodando
- **500 Internal Error**: Verifique logs e banco de dados

## 🎯 Teste Completo em 5 Minutos

1. **Importar** collection e environment
2. **Iniciar** os 3 serviços
3. **Executar** Register User → Login
4. **Criar** 3 produtos diferentes
5. **Fazer** 2 pedidos
6. **Verificar** listas de produtos e pedidos

✨ **Pronto! Seu e-commerce está funcionando!** ✨
