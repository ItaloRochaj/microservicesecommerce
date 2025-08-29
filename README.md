# 🛍️ Microserviços E-commerce

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=.net)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?style=flat&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.13-FF6600?style=flat&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker-Container-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)

Projeto final desenvolvido no Bootcamp Avanade - Back-end com .NET e IA, com foco em práticas modernas de desenvolvimento de software. Trata-se de um sistema completo de e-commerce construído com arquitetura de microserviços em .NET 8, utilizando RabbitMQ para comunicação assíncrona entre serviços, JWT para autenticação e MySQL para persistência de dados.

---
## 📋 Índice

[🏗️ Arquitetura](#️-arquitetura)  
[🧩 Componentes](#-componentes)  
[🔧 Tecnologias](#-tecnologias)  
[🚀 Como Executar](#-como-executar)  
[🗄️ Banco de Dados](#️-banco-de-dados)  
[🔌 API Endpoints](#-api-endpoints)  
[🐰 RabbitMQ](#-rabbitmq)  
[🧪 Testes](#-testes)  
[📁 Estrutura do Projeto](#-estrutura-do-projeto)  
[📚 Documentação Adicional](#-documentação-adicional)  

---
## 🏗️ Arquitetura

### Diagrama da Arquitetura
```mermaid
graph TB
    Client[👤 Cliente]
    Gateway[🌐 API Gateway<br/>Porta 5000<br/>JWT Auth]
    Stock[📦 Stock Service<br/>Porta 5001<br/>Gestão de Produtos]
    Sales[🛒 Sales Service<br/>Porta 5002<br/>Gestão de Pedidos]
    
    subgraph "💾 Bancos de Dados"
        AuthDB[(🔐 study_projects<br/>Usuários)]
        StockDB[(📦 stock_db<br/>Produtos)]
        SalesDB[(🛒 sales_db<br/>Pedidos)]
    end
    
    subgraph "📬 Mensageria"
        RabbitMQ[🐰 RabbitMQ<br/>Porta 5672/15672]
        OrderQueue[📋 order-created]
        StockQueue[📦 stock-update]
    end
    
    Client --> Gateway
    Gateway --> Stock
    Gateway --> Sales
    
    Gateway --> AuthDB
    Stock --> StockDB
    Sales --> SalesDB
    
    Sales --> RabbitMQ
    Stock --> RabbitMQ
    RabbitMQ --> OrderQueue
    RabbitMQ --> StockQueue
    
    style Gateway fill:#e1f5fe
    style Stock fill:#f3e5f5
    style Sales fill:#e8f5e8
    style RabbitMQ fill:#fff3e0
```

---
### Fluxo de Comunicação  
**📱 Cliente** → Autentica via **API Gateway**  
**🌐 Gateway** → Roteia requisições para microserviços  
**🛒 Sales Service** → Cria pedido e publica mensagem no RabbitMQ  
**📦 Stock Service** → Consome mensagem e atualiza estoque  
**🔄 Comunicação assíncrona** via filas RabbitMQ  
**🎪 Health Dashboard** → Monitora todos os serviços em tempo real  

### 🎯 Monitoramento Inteligente
O sistema inclui **Health Checks Inteligentes** que monitoram:    
✅ **Status dos Microserviços** (StockService, SalesService)  
✅ **Infraestrutura** (RabbitMQ, MySQL, Sistema)  
✅ **Métricas** (Memória, Disco, Response Time)  
✅ **Dashboard Visual** com atualização automática  
✅ **API JSON** para integração externa

---
## 🧩 Componentes

### 🌐 API Gateway
- **Responsabilidade**: Ponto de entrada único, autenticação, roteamento
- **Porta**: 5000
- **Funcionalidades**:  
✅ Autenticação JWT  
✅ Roteamento via YARP  
✅ CORS habilitado  
✅ Swagger UI  
✅ **Health Checks Dashboard** 🎪  
✅ **Monitoramento em Tempo Real**  
✅ **API de Health Consolidada**  

### 📦 Stock Service
- **Responsabilidade**: Gerenciamento de produtos e estoque
- **Porta**: 5001
- **Funcionalidades**:  
✅ CRUD de produtos  
✅ Controle de estoque  
✅ Validação de disponibilidade  
✅ Consumo de mensagens RabbitMQ  
✅ Logs estruturados (Serilog)  

### 🛒 Sales Service
- **Responsabilidade**: Gerenciamento de vendas e pedidos
- **Porta**: 5002
- **Funcionalidades**:  
✅ CRUD de pedidos  
✅ Cálculo de totais  
✅ Publicação de eventos RabbitMQ  
✅ Validação de dados  
✅ Logs estruturados (Serilog)  

### 📚 Shared Library
- **Responsabilidade**: Modelos e utilitários compartilhados
- **Contém**:  
✅ Models (User, Product, Order)  
✅ DTOs  
✅ Enums (OrderStatus)  
✅ Interfaces compartilhadas

---
## 🔧 Tecnologias

### Backend Framework
**🔹 .NET 8** - Framework principal  
**🔹 ASP.NET Core** - Web API  
**🔹 Entity Framework Core** - ORM  
**🔹 AutoMapper** - Mapeamento de objetos  

### Banco de Dados
**🔹 MySQL 8.0** - Banco principal  
**🔹 Pomelo.EntityFrameworkCore.MySQL** - Provider EF Core  

### Mensageria
**🔹 RabbitMQ 3.13** - Message Broker  
**🔹 RabbitMQ.Client** - Cliente .NET  

### Autenticação & Segurança
**🔹 JWT Bearer** - Autenticação stateless  
**🔹 BCrypt** - Hash de senhas  

### API Gateway
**🔹 YARP (Yet Another Reverse Proxy)** - Proxy reverso  
**🔹 Microsoft.AspNetCore.Authentication.JwtBearer**  

### Documentação & Testes
**🔹 Swagger/OpenAPI** - Documentação automática  
**🔹 xUnit** - Framework de testes  
**🔹 Moq** - Mocking para testes  

### Logs & Monitoramento
**🔹 Serilog** - Logging estruturado  
**🔹 Health Checks** - Monitoramento de saúde  
**🔹 Health Dashboard** - Interface visual de monitoramento 🎪  
**🔹 Health API JSON** - Integração programática  
**🔹 Real-time Monitoring** - Atualização automática  

### Containerização
**🔹 Docker** - Containerização  
**🔹 Docker Compose** - Orquestração local  

---
## 🚀 Como Executar

### 🔧 Pré-requisitos
Certifique-se de ter instalado:  
✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)  
✅ [MySQL 8.0+](https://dev.mysql.com/downloads/mysql/) ou Docker  
✅ [Docker Desktop](https://www.docker.com/products/docker-desktop) (recomendado)  

### 🐳 Opção 1: Usando Docker (Recomendado)
```bash
# 1. Clone o repositório
git clone <repository-url>
cd microservicesecommerce

# 2. Inicie os containers (MySQL + RabbitMQ)
docker-compose up -d

# 3. Execute o script de inicialização completa
.\start-system.bat
# ou no PowerShell:
.\start-system.ps1
```

### 🔧 Opção 2: Instalação Manual
```bash
# 1. Clone o repositório
git clone <repository-url>
cd microservicesecommerce

# 2. Configure MySQL (se não usar Docker)
.\scripts\setup-mysql.ps1

# 3. Inicie apenas RabbitMQ via Docker
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# 4. Execute os serviços
.\scripts\start-services-mysql.ps1
```

### ▶️ Opção 3: Comandos Rápidos (Health Dashboard)

**Para iniciar rapidamente com Health Dashboard:**
```powershell
# 1. Iniciar RabbitMQ
docker run -d --name rabbitmq-microservices -p 5672:5672 -p 15672:15672 rabbitmq:3-management

# 2. Iniciar todos os serviços (3 terminais separados)
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd D:\GitHub\microservicesecommerce\src\ApiGateway; dotnet run"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd D:\GitHub\microservicesecommerce\src\StockService; dotnet run"  
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd D:\GitHub\microservicesecommerce\src\SalesService; dotnet run"

# 3. Acessar o Health Dashboard
# http://localhost:5000/api/health/dashboard
```

### 🌟 Verificação da Instalação
Após a execução, verifique se os serviços estão rodando:

| Serviço | URL | Status |
|---------|-----|---------|
| � **Health Dashboard** | http://localhost:5000/api/health/dashboard | ✅ |
| 📦 **Stock Service** | http://localhost:5001/swagger | ✅ |
| 🛒 **Sales Service** | http://localhost:5002/swagger | ✅ |
| 🐰 **RabbitMQ Management** | http://localhost:15672 | ✅ |
| 📊 **Health API (JSON)** | http://localhost:5000/api/health/consolidated | ✅ |

**🔑 Credenciais:**
- **RabbitMQ**: `guest` / `guest`
- **Health Dashboard**: Acesso direto (sem login)

---
## 🗄️ Banco de Dados

### 📊 Configuração Geral
**Credenciais MySQL:**  
🔑 **Usuário**: `developer`  
🔐 **Senha**: `Luke@2020`  
🌐 **Host**: `localhost`  
🔌 **Porta**: `3306`  

### 🗃️ Estrutura dos Bancos

#### 🔐 study_projects (API Gateway)
```sql
-- Tabela de usuários para autenticação
Users
├── Id (int, PK, Auto Increment)
├── Username (varchar(50), Unique)
├── Email (varchar(100), Unique)
├── PasswordHash (varchar(255))
├── FullName (varchar(100))
├── CreatedAt (datetime)
└── UpdatedAt (datetime)
```

#### 📦 stock_db (Stock Service)
```sql
-- Tabela de produtos
Products
├── Id (int, PK, Auto Increment)
├── Name (varchar(100))
├── Description (text)
├── Price (decimal(10,2))
├── StockQuantity (int)
├── Category (varchar(50))
├── CreatedAt (datetime)
└── UpdatedAt (datetime)
```

#### 🛒 sales_db (Sales Service)
```sql
-- Tabela de pedidos
Orders
├── Id (int, PK, Auto Increment)
├── CustomerId (int)
├── CustomerName (varchar(100))
├── CustomerEmail (varchar(200))
├── TotalAmount (decimal(10,2))
├── Status (int) -- enum OrderStatus
├── OrderDate (datetime)
├── CreatedAt (datetime)
└── UpdatedAt (datetime)

-- Tabela de itens do pedido
OrderItems
├── Id (int, PK, Auto Increment)
├── OrderId (int, FK → Orders.Id)
├── ProductId (int)
├── ProductName (varchar(100))
├── Quantity (int)
├── UnitPrice (decimal(10,2))
└── TotalPrice (decimal(10,2))
```

### 🔄 Migrations
As migrações são executadas automaticamente na inicialização dos serviços.

**Para executar manualmente:**
```bash
# Stock Service
cd src/StockService
dotnet ef database update

# Sales Service
cd ../SalesService
dotnet ef database update

# API Gateway
cd ../ApiGateway
dotnet ef database update
```

**Para criar nova migration:**
```bash
# Exemplo para Stock Service
cd src/StockService
dotnet ef migrations add NomeDaMigracao
```

---
## 🔌 API Endpoints

### 🔐 Autenticação

#### POST `/api/auth/login`
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Resposta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-08-27T12:00:00Z",
  "user": {
    "id": 1,
    "username": "admin",
    "fullName": "Administrador",
    "email": "admin@test.com"
  }
}
```

#### POST `/api/auth/register`
```json
{
  "username": "novouser",
  "email": "user@example.com",
  "password": "senha123",
  "fullName": "Nome Completo"
}
```

### 📦 Produtos (via Gateway)

#### GET `/api/products`
Lista todos os produtos disponíveis.

**Headers:**
```
Authorization: Bearer {token}
```

**Resposta:**
```json
[
  {
    "id": 1,
    "name": "iPhone 15",
    "description": "Smartphone Apple",
    "price": 4500.00,
    "stockQuantity": 50,
    "category": "Eletrônicos"
  }
]
```

#### POST `/api/products`
Cria um novo produto.

```json
{
  "name": "Samsung Galaxy S24",
  "description": "Smartphone Samsung",
  "price": 3999.99,
  "stockQuantity": 30,
  "category": "Eletrônicos"
}
```

#### PUT `/api/products/{id}`
Atualiza um produto existente.

#### DELETE `/api/products/{id}`
Remove um produto.

### 🛒 Pedidos (via Gateway)

#### GET `/api/orders`
Lista todos os pedidos.

**Headers:**
```
Authorization: Bearer {token}
```

#### POST `/api/orders`
Cria um novo pedido.

```json
{
  "customerId": 6,
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

**Resposta:**
```json
{
  "id": 10,
  "customerId": 6,
  "customerName": "João Silva",
  "customerEmail": "joao@email.com",
  "totalAmount": 13499.97,
  "status": 0,
  "orderDate": "2025-08-26T23:45:00Z",
  "items": [
    {
      "id": 15,
      "productId": 1,
      "productName": "iPhone 15",
      "quantity": 2,
      "unitPrice": 4500.00,
      "totalPrice": 9000.00
    }
  ]
}
```

#### GET `/api/orders/{id}`
Consulta um pedido específico.

#### PUT `/api/orders/{id}/status`
Atualiza o status de um pedido.

```json
{
  "status": 1  // 0=Pending, 1=Confirmed, 2=Shipped, 3=Delivered, 4=Cancelled
}
```

---
### 🏥 Health Checks & Dashboard Inteligente

#### 🎪 **Microservices Health Dashboard**
Sistema avançado de monitoramento em tempo real com interface visual moderna.

**🌟 Acesso ao Dashboard:**
```
http://localhost:5000/api/health/dashboard
```

#### 🔍 **Features do Health Dashboard:**  
✅ **Status em tempo real** de todos os microserviços  
✅ **Métricas de sistema** (CPU, memória, disco)  
✅ **Monitoramento RabbitMQ** com status de filas  
✅ **Interface responsiva** com design moderno  
✅ **Atualização automática** a cada 30 segundos  
✅ **API JSON** para integração  
✅ **Alertas visuais** por cores (Verde/Amarelo/Vermelho)  

#### 📈 **Componentes Monitorados:**
| Componente | Descrição | Thresholds |
|------------|-----------|------------|
| 🟢 **Memory** | Uso de memória | < 200MB (Healthy), < 500MB (Degraded) |
| 💾 **DiskSpace** | Espaço em disco | < 80% (Healthy), < 90% (Degraded) |
| 🐰 **RabbitMQ** | Message broker | Conectividade e filas ativas |
| 📦 **StockService** | Serviço de produtos | Response time < 1s |
| 🛒 **SalesService** | Serviço de pedidos | Response time < 1s |

#### 🔧 **Health Check APIs:**
```bash
# Dashboard visual (HTML)
GET http://localhost:5000/api/health/dashboard

# Status consolidado (JSON)
GET http://localhost:5000/api/health/consolidated

# Health checks individuais
GET http://localhost:5000/health (Gateway)
GET http://localhost:5001/health (Stock)
GET http://localhost:5002/health (Sales)
```

#### 📊 **Exemplo Response JSON:**
```json
{
  "overallStatus": "Healthy",
  "timestamp": "2025-08-28T21:47:07Z",
  "services": {
    "StockService": {
      "status": "Healthy",
      "responseTimeMs": 119,
      "description": "Service is healthy"
    },
    "SalesService": {
      "status": "Healthy", 
      "responseTimeMs": 155,
      "description": "Service is healthy"
    },
    "RabbitMQ": {
      "status": "Healthy",
      "responseTimeMs": 39,
      "description": "Message broker is healthy"
    }
  },
  "summary": {
    "healthy": 4,
    "degraded": 0,
    "unhealthy": 1,
    "healthPercentage": 80
  }
}
```

---
## 🐰 RabbitMQ

### 📋 Configuração
**Conexão:**  
🌐 **Host**: `localhost`  
🔌 **Porta AMQP**: `5672`  
🖥️ **Management UI**: `15672`  
🔑 **Usuário**: `guest`  
🔐 **Senha**: `guest`  

### 📬 Filas Implementadas

#### 📋 order-created
- **Publisher**: Sales Service
- **Consumer**: Stock Service
- **Finalidade**: Notifica criação de novo pedido
- **Payload**:
```json
{
  "OrderId": 10,
  "UserId": "6",
  "Items": [
    {
      "ProductId": 1,
      "Quantity": 2,
      "UnitPrice": 4500.00
    }
  ],
  "TotalAmount": 9000.00,
  "Timestamp": "2025-08-26T23:45:00Z"
}
```

#### 📦 stock-update
- **Publisher**: Stock Service
- **Consumer**: Sales Service (futuro)
- **Finalidade**: Notifica atualizações de estoque
- **Payload**:
```json
{
  "ProductId": 1,
  "ProductName": "iPhone 15",
  "OldQuantity": 50,
  "NewQuantity": 48,
  "Operation": "DECREASE",
  "Timestamp": "2025-08-26T23:45:00Z"
}
```

---
### 🔄 Fluxo de Mensagens  
**🛒 Cliente cria pedido** → Sales Service  
**📝 Sales Service** → Persiste pedido no banco  
**📤 Sales Service** → Publica mensagem na fila `order-created`  
**📥 Stock Service** → Consome mensagem da fila  
**📦 Stock Service** → Atualiza estoque dos produtos  
**📤 Stock Service** → Publica mensagem na fila `stock-update` (opcional)  

### 🛠️ Ferramentas de Debug

#### Script PowerShell para Consumo Manual:
```powershell
# Execute na raiz do projeto
.\RabbitMQ-Consumer-Test.ps1
```

#### Interface Web:
- **URL**: http://localhost:15672
- **Login**: guest / guest
- Navegue para **Queues** → Selecione fila → **Get messages**

---
## 🧪 Testes

### 🗂️ Estrutura de Testes
```
tests/
├── StockService.Tests/    # Testes do serviço de estoque
│   ├── Controllers/       # Testes dos controllers
│   ├── Services/          # Testes dos services
│   └── Integration/       # Testes de integração
└── SalesService.Tests/    # Testes do serviço de vendas
    ├── Controllers/       # Testes dos controllers
    ├── Services/          # Testes dos services
    └── Integration/       # Testes de integração
```

### 🚀 Executando Testes
```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
dotnet test tests/StockService.Tests/

# Executar com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"

# Script automatizado
.\scripts\run-tests.ps1
```

### 📊 Tipos de Testes

#### 🔬 Testes Unitários  
✅ Controllers  
✅ Services  
✅ Repositories  
✅ Validators  
✅ Mappers  

#### 🔗 Testes de Integração
✅ APIs completas  
✅ Banco de dados  
✅ RabbitMQ  
✅ Autenticação  

#### 🧪 Exemplo de Teste
```csharp
[Fact]
public async Task CreateOrder_ValidRequest_ReturnsCreatedOrder()
{
    // Arrange
    var request = new CreateOrderRequest
    {
        CustomerId = 1,
        CustomerName = "Test User",
        CustomerEmail = "test@example.com",
        Items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 2 }
        }
    };

    // Act
    var result = await _orderService.CreateOrderAsync(request);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(request.CustomerName, result.CustomerName);
    Assert.Equal(1, result.Items.Count);
}
```

---
## 📁 Estrutura do Projeto
```
microservicesecommerce/
├── 📁 src/                             # Código fonte
│   ├── 🌐 ApiGateway/                  # API Gateway
│   │   ├── Controllers/                # AuthController
│   │   ├── Data/                       # DbContext para autenticação
│   │   ├── Services/                   # AuthService, IAuthService
│   │   ├── Properties/                 # launchSettings.json
│   │   ├── appsettings.json            # Configurações (JWT, DB, YARP)
│   │   └── Program.cs                  # Configuração da aplicação
│   │
│   ├── 📦 StockService/                # Serviço de Estoque
│   │   ├── Controllers/                # ProductsController
│   │   ├── Data/                       # StockDbContext
│   │   ├── Services/                   # ProductService, RabbitMQService
│   │   ├── Consumers/                  # OrderCreatedConsumer
│   │   ├── Migrations/                 # Migrações EF Core
│   │   ├── logs/                       # Arquivos de log
│   │   ├── appsettings.json            # Configurações (DB, RabbitMQ)
│   │   └── Program.cs                  # Configuração + Background Services
│   │
│   ├── 🛒 SalesService/               # Serviço de Vendas
│   │   ├── Controllers/               # OrdersController
│   │   ├── Data/                      # SalesDbContext
│   │   ├── Services/                  # OrderService, RabbitMQPublisher
│   │   ├── Migrations/                # Migrações EF Core
│   │   ├── logs/                      # Arquivos de log
│   │   ├── appsettings.json           # Configurações (DB, RabbitMQ)
│   │   └── Program.cs                 # Configuração da aplicação
│   │
│   └── 📚 Shared/                    # Biblioteca compartilhada
│       ├── Models/                    # User, Product, Order, OrderItem
│       ├── DTOs/                      # Data Transfer Objects
│       └── Enums/                     # OrderStatus
│
├── 📁 tests/                         # Projetos de teste
│   ├── 🧪 StockService.Tests/        # Testes do StockService
│   │   ├── Controllers/              # Testes dos controllers
│   │   ├── Services/                 # Testes dos services
│   │   └── Integration/              # Testes de integração
│   │
│   └── 🧪 SalesService.Tests/        # Testes do SalesService
│       ├── Controllers/              # Testes dos controllers
│       ├── Services/                 # Testes dos services
│       └── Integration/              # Testes de integração
│
├── 📄 docker-compose.yml             # MySQL + RabbitMQ
├── 📄 MicroservicesEcommerce.sln     # Solution do Visual Studio
├── 📄 global.json                    # Versão do .NET SDK

```

### 🎯 Arquivos de Configuração Principais

#### appsettings.json (Exemplo - API Gateway)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=study_projects;Uid=developer;Pwd=Luke@2020;",
    "RabbitMQ": "amqp://guest:guest@localhost:5672/"
  },
  "Jwt": {
    "Key": "MinhaChaveSecretaSuperSeguraComPeloMenos32Caracteres!",
    "Issuer": "MicroservicesEcommerce",
    "Audience": "MicroservicesEcommerce"
  },
  "ReverseProxy": {
    "Routes": {
      "products": {
        "ClusterId": "stock",
        "Match": { "Path": "/api/products/{**remainder}" }
      },
      "orders": {
        "ClusterId": "sales", 
        "Match": { "Path": "/api/orders/{**remainder}" }
      }
    },
    "Clusters": {
      "stock": {
        "Destinations": {
          "stock1": { "Address": "http://localhost:5001/" }
        }
      },
      "sales": {
        "Destinations": {
          "sales1": { "Address": "http://localhost:5002/" }
        }
      }
    }
  }
}
```

---
## 📚 Documentação Adicional

### 📖 Arquivos de Documentação

| Arquivo | Descrição |
|---------|-----------|
| [Health-Dashboard.md](https://github.com/ItaloRochaj/microservicesecommerce/blob/main/Documenta%C3%A7%C3%A3o%20Adicional/Health-Dashboard.md) | Documentação completa do Health Dashboard | 
| [Troubleshooting.md`](https://github.com/ItaloRochaj/microservicesecommerce/blob/main/Documenta%C3%A7%C3%A3o%20Adicional/Troubleshooting.md) | Documentação para Erros |

---
### 🌍 Environments
- **`Microservices-Ecommerce-COMPLETO.postman_environment.json`**
- **`Microservices-Ecommerce.postman_environment.json`**
---

## 🚀 Deploy e Produção

### 🐳 Docker Compose Completo
Para deploy completo com todos os serviços:
```yaml
version: '3.8'
services:
  mysql:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: Luke@2020
      MYSQL_USER: developer
      MYSQL_PASSWORD: Luke@2020
    ports:
      - "3306:3306"
    volumes:
      - mysql_data:/var/lib/mysql

  rabbitmq:
    image: rabbitmq:3-management
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
    ports:
      - "5672:5672"
      - "15672:15672"
    volumes:
      - rabbitmq_data:/var/lib/rabbitmq

  # Adicione aqui os serviços .NET quando dockerizados
```

---
### 📊 Monitoramento

#### Health Checks Implementados
✅ Database connectivity  
✅ RabbitMQ connectivity    
✅ Service availability  

#### Logs Estruturados
✅ Serilog com formatação JSON  
✅ Logs salvos em arquivos por serviço  
✅ Níveis configuráveis (Info, Warning, Error)  

---
## 🔧 Configurações Avançadas

### 🔐 Segurança

#### JWT Configuration
```json
{
  "Jwt": {
    "Key": "MinhaChaveSecretaSuperSeguraComPeloMenos32Caracteres!",
    "Issuer": "MicroservicesEcommerce",
    "Audience": "MicroservicesEcommerce",
    "ExpiryMinutes": 60
  }
}
```

#### CORS Policy
```csharp
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

### 🔄 RabbitMQ Configuração Avançada

#### Connection Factory
```csharp
var factory = new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    VirtualHost = "/",
    AutomaticRecoveryEnabled = true,
    RequestedHeartbeat = TimeSpan.FromSeconds(30)
};
```

#### Queue Declaration
```csharp
channel.QueueDeclare(
    queue: "order-created",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);
```

### 🔍 Logs de Debug

#### Localização dos Logs
- **Stock Service**: `src/StockService/logs/`
- **Sales Service**: `src/SalesService/logs/`
- **API Gateway**: Console output

#### Verificação de Logs
```bash
# Tail dos logs em tempo real
Get-Content "src/StockService/logs/stock-service-*.log" -Wait
```

### 📝 Padrões de Código
✅ Use **PascalCase** para classes e métodos  
✅ Use **camelCase** para variáveis locais  
✅ Adicione **XML comments** em métodos públicos  
✅ Escreva **testes unitários** para nova funcionalidade  
✅ Siga as **convenções .NET**

---
### 👨🏻‍💻 Autor:
<table style="border=0">
  <tr>
    <td align="left">
      <a href="https://github.com/ItaloRochaj">
        <span><b>Italo Rocha</b></span>
      </a>
      <br>
      <span>Full-Stack Development</span>
    </td>
  </tr>
</table>
