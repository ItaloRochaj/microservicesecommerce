using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockService.Services;
using Shared.Models;

namespace StockService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Produto com ID {id} não encontrado");

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct == null)
                return NotFound($"Produto com ID {id} não encontrado");

            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        try
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound($"Produto com ID {id} não encontrado");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/check-stock")]
    public async Task<ActionResult<bool>> CheckStock(int id, [FromBody] int quantity)
    {
        try
        {
            var available = await _productService.CheckStockAvailabilityAsync(id, quantity);
            return Ok(new { ProductId = id, Quantity = quantity, Available = available });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar estoque do produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Endpoint público para comunicação entre serviços (sem autenticação)
    [HttpPost("internal/{id}/check-stock")]
    [AllowAnonymous]
    public async Task<ActionResult<bool>> InternalCheckStock(int id, [FromBody] int quantity)
    {
        try
        {
            var available = await _productService.CheckStockAvailabilityAsync(id, quantity);
            return Ok(new { ProductId = id, Quantity = quantity, Available = available });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar estoque interno do produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Endpoint público para obter produto (sem autenticação)
    [HttpGet("internal/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Product>> InternalGetProduct(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Produto com ID {id} não encontrado");

            return Ok(product);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto interno {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("{id}/update-stock")]
    public async Task<ActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        try
        {
            var updated = await _productService.UpdateStockAsync(id, quantity);
            if (!updated)
                return BadRequest("Não foi possível atualizar o estoque");

            return Ok(new { ProductId = id, QuantityChanged = quantity, Message = "Estoque atualizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estoque do produto {ProductId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}
