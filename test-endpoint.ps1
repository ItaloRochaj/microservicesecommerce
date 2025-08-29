# Teste do endpoint de update status corrigido
Write-Host "=== TESTE FINAL DO ENDPOINT CORRIGIDO ===" -ForegroundColor Cyan

# JSON para teste
$body = @{
    status = "Shipped"
} | ConvertTo-Json

Write-Host "Endpoint: PUT http://localhost:5002/api/orders/internal/1/status" -ForegroundColor Yellow
Write-Host "Body: $body" -ForegroundColor Yellow

try {
    $result = Invoke-RestMethod -Uri "http://localhost:5002/api/orders/internal/1/status" -Method PUT -ContentType "application/json" -Body $body
    Write-Host "✅ SUCESSO! Correção implementada com êxito!" -ForegroundColor Green
    Write-Host "Resposta:" -ForegroundColor Green
    $result | ConvertTo-Json -Depth 3
} catch {
    Write-Host "❌ Erro: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}
