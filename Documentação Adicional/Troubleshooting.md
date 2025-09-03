## 🐛 Troubleshooting

### ❗ Problemas Comuns

#### 1. Erro de Conexão MySQL
```
Solution: Verifique se o MySQL está rodando na porta 3306
Command: docker ps | grep mysql
```

#### 2. Erro de Conexão RabbitMQ
```
Solution: Verifique se o RabbitMQ está acessível
URL: http://localhost:15672
Login: guest/guest
```

#### 3. Erro 502 Bad Gateway
```
Solution: Verifique se todos os serviços estão rodando
Commands:
- netstat -ano | findstr ":5000"  # Gateway
- netstat -ano | findstr ":5001"  # Stock  
- netstat -ano | findstr ":5002"  # Sales
```

#### 4. Erro de Migração
```bash
# Recrie as migrações
cd src/StockService
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```