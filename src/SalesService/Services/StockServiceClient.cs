using System.Text.Json;

namespace SalesService.Services;

public interface IStockServiceClient
{
    Task<bool> CheckStockAvailabilityAsync(int productId, int quantity);
    Task<ProductDto?> GetProductAsync(int productId);
}

public class StockServiceClient : IStockServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StockServiceClient> _logger;

    public StockServiceClient(HttpClient httpClient, ILogger<StockServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckStockAvailabilityAsync(int productId, int quantity)
    {
        try
        {
            _logger.LogInformation("Chamando check-stock interno para produto {ProductId} com quantidade {Quantity}", productId, quantity);
            var response = await _httpClient.PostAsJsonAsync(
                $"/api/products/internal/{productId}/check-stock", 
                quantity);

            _logger.LogInformation("Status da resposta check-stock: {StatusCode}", response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Conteúdo da resposta check-stock: {Content}", content);
                var result = JsonSerializer.Deserialize<StockCheckResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                _logger.LogInformation("Resultado deserializado: Available={Available}", result?.Available);
                return result?.Available ?? false;
            }

            _logger.LogWarning("Resposta de check-stock não foi bem-sucedida: {StatusCode}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar estoque do produto {ProductId}", productId);
            return false;
        }
    }

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/products/internal/{productId}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto {ProductId}", productId);
            return null;
        }
    }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class StockCheckResult
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public bool Available { get; set; }
}
