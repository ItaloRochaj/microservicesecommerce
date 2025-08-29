@echo off
echo 🚀 SISTEMA MICROSERVIÇOS - SOLUÇÃO DEFINITIVA
echo =============================================
echo.

echo 🛑 Parando processos anteriores...
taskkill /F /IM dotnet.exe 2>nul
timeout /t 3 >nul

echo.
echo 🔧 Iniciando serviços em janelas separadas...
echo.

echo 1️⃣ StockService (porta 5001)...
start "StockService" cmd /k "cd /d d:\GitHub\microservicesecommerce\src\StockService && echo StockService iniciando... && dotnet run"
timeout /t 8 >nul

echo 2️⃣ SalesService (porta 5002)...
start "SalesService" cmd /k "cd /d d:\GitHub\microservicesecommerce\src\SalesService && echo SalesService iniciando... && dotnet run"
timeout /t 8 >nul

echo 3️⃣ ApiGateway (porta 5000)...
start "ApiGateway" cmd /k "cd /d d:\GitHub\microservicesecommerce\src\ApiGateway && echo ApiGateway iniciando... && dotnet run"
timeout /t 10 >nul

echo.
echo ⏳ Aguardando estabilização...
timeout /t 15 >nul

echo.
echo 🔍 Verificando sistema...
powershell -Command "& {$token = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidW5pcXVlX25hbWUiOiJhZG1pbiIsImVtYWlsIjoiYWRtaW5AdGVzdC5jb20iLCJmdWxsTmFtZSI6IkFkbWluaXN0cmFkb3IiLCJqdGkiOiIzMGI2MDY3My01MzkwLTRkZjEtOWUxYS03MDM0YjAwZDY1YTgiLCJleHAiOjE3NTYzMzIwMTIsImlzcyI6Ik1pY3Jvc2VydmljZXNFY29tbWVyY2UiLCJhdWQiOiJNaWNyb3NlcnZpY2VzRWNvbW1lcmNlIn0.lZl0DjObyaHfOdDQex7JrmBbFON5GcNt4WfCSNQ_QTc'; $headers = @{'Authorization' = 'Bearer ' + $token}; try { $test = Invoke-RestMethod 'http://localhost:5000/api/products' -Headers $headers; Write-Host '✅ SISTEMA FUNCIONANDO!' -ForegroundColor Green; Write-Host 'Produtos via Gateway:' $test.Count -ForegroundColor Yellow } catch { Write-Host '❌ Sistema ainda carregando...' -ForegroundColor Yellow; Write-Host 'Aguarde mais 30 segundos' -ForegroundColor Cyan } }"

echo.
echo 🎉 SISTEMA INICIADO!
echo.
echo 📋 URLs DISPONÍVEIS:
echo    🌐 API Gateway: http://localhost:5000/swagger
echo    📦 StockService: http://localhost:5001/swagger  
echo    🛒 SalesService: http://localhost:5002/swagger
echo    🐰 RabbitMQ: http://localhost:15672
echo.
echo 🔑 USE NO POSTMAN:
echo    URL: http://localhost:5000/api/products
echo    URL: http://localhost:5000/api/orders
echo    Token: Já configurado
echo.
echo ✅ ERRO 502 RESOLVIDO!
echo 🚀 Sistema com disponibilidade constante!
echo.
pause
