# ========================================
# 🐰 SCRIPT PARA CONSUMIR MENSAGENS RABBITMQ
# ========================================

Write-Host "🐰 RABBITMQ MESSAGE CONSUMER" -ForegroundColor Green
Write-Host "============================" -ForegroundColor Green
Write-Host ""

# Função para consumir mensagem da fila order-created
function Get-OrderCreatedMessage {
    Write-Host "📋 Consumindo mensagem da fila 'order-created'..." -ForegroundColor Cyan
    
    try {
        # Credenciais RabbitMQ (guest:guest em base64)
        $auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("guest:guest"))
        $headers = @{"Authorization" = "Basic $auth"}
        
        # Consumir mensagem da fila
        $response = Invoke-RestMethod -Uri "http://localhost:15672/api/queues/%2f/order-created/get" -Method POST -Headers $headers -ContentType "application/json" -Body '{"count":1,"ackmode":"ack_requeue_false","encoding":"auto"}'
        
        if ($response -and $response.Count -gt 0) {
            $message = $response[0]
            Write-Host "✅ MENSAGEM CONSUMIDA COM SUCESSO!" -ForegroundColor Green
            Write-Host ""
            Write-Host "📋 DETALHES DA MENSAGEM:" -ForegroundColor Yellow
            Write-Host "🔢 Message Count: $($message.message_count)" -ForegroundColor Blue
            Write-Host "📦 Payload Bytes: $($message.payload_bytes)" -ForegroundColor Blue
            Write-Host "🔄 Redelivered: $($message.redelivered)" -ForegroundColor Blue
            Write-Host "📊 Exchange: $($message.exchange)" -ForegroundColor Blue
            Write-Host "🏷️ Routing Key: $($message.routing_key)" -ForegroundColor Blue
            Write-Host ""
            Write-Host "📄 CONTEÚDO DA MENSAGEM:" -ForegroundColor Yellow
            Write-Host $message.payload -ForegroundColor White
            Write-Host ""
            
            # Parse do JSON se possível
            try {
                $orderData = $message.payload | ConvertFrom-Json
                Write-Host "🛒 DADOS DO PEDIDO PROCESSADOS:" -ForegroundColor Green
                Write-Host "🆔 Order ID: $($orderData.OrderId)" -ForegroundColor Cyan
                Write-Host "👤 Customer: $($orderData.CustomerName)" -ForegroundColor Cyan
                Write-Host "📧 Email: $($orderData.CustomerEmail)" -ForegroundColor Cyan
                Write-Host "💰 Total: R$ $($orderData.TotalAmount)" -ForegroundColor Green
                Write-Host "📦 Items: $($orderData.Items.Count)" -ForegroundColor Yellow
            } catch {
                Write-Host "💡 Conteúdo não é JSON válido ou estrutura diferente" -ForegroundColor Yellow
            }
        } else {
            Write-Host "📭 Nenhuma mensagem na fila 'order-created'" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "❌ Erro ao consumir mensagem: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Função para verificar status das filas
function Get-QueueStatus {
    Write-Host "📊 Verificando status de todas as filas..." -ForegroundColor Cyan
    
    try {
        $auth = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("guest:guest"))
        $headers = @{"Authorization" = "Basic $auth"}
        
        $queues = Invoke-RestMethod -Uri "http://localhost:15672/api/queues" -Headers $headers
        
        Write-Host ""
        Write-Host "📋 STATUS DAS FILAS:" -ForegroundColor Yellow
        Write-Host "====================" -ForegroundColor Yellow
        
        foreach ($queue in $queues) {
            $status = if ($queue.messages -gt 0) { "🟢 COM MENSAGENS" } else { "⚪ VAZIA" }
            $consumers = if ($queue.consumers -gt 0) { "👥 $($queue.consumers) consumidores" } else { "👤 Sem consumidores" }
            
            Write-Host "📁 $($queue.name): $($queue.messages) msgs - $status - $consumers" -ForegroundColor Cyan
        }
    } catch {
        Write-Host "❌ Erro ao verificar filas: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Executar funções
Write-Host "🔍 1. Verificando status das filas..." -ForegroundColor Blue
Get-QueueStatus

Write-Host ""
Write-Host "🔍 2. Consumindo mensagem da fila order-created..." -ForegroundColor Blue
Get-OrderCreatedMessage

Write-Host ""
Write-Host "🔍 3. Verificando status após consumo..." -ForegroundColor Blue
Get-QueueStatus

Write-Host ""
Write-Host "✅ PROCESSO CONCLUÍDO!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 PRÓXIMOS PASSOS:" -ForegroundColor Yellow
Write-Host "   1. Acesse http://localhost:15672 para monitorar visualmente" -ForegroundColor Cyan
Write-Host "   2. Crie novos pedidos para gerar mais mensagens" -ForegroundColor Cyan
Write-Host "   3. Observe o processamento automático pelo StockService" -ForegroundColor Cyan
