using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesService.Services;
using Shared.Models;
using System.Security.Claims;
using static SalesService.Services.OrderService;

namespace SalesService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        try
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByUserAsync(userId);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedidos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            var userId = GetCurrentUserId();
            if (order.CustomerId.ToString() != userId)
                return Forbid("Você não tem permissão para visualizar este pedido");

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.Items == null || !request.Items.Any())
                return BadRequest("O pedido deve conter pelo menos um item");

            var order = await _orderService.CreateOrderAsync(request);

            return CreatedAtAction(nameof(GetOrder), new { id = order!.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Falha ao criar pedido - regra de negócio");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<Order>> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status é obrigatório");

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
                return BadRequest($"Status '{request.Status}' é inválido. Valores válidos: Pending, Processing, Shipped, Delivered, Cancelled");

            var order = await _orderService.UpdateOrderStatusAsync(id, status);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("internal/{id}/status")]
    [AllowAnonymous]
    public async Task<ActionResult<Order>> UpdateOrderStatusInternal(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Status))
                return BadRequest("Status é obrigatório");

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
                return BadRequest($"Status '{request.Status}' é inválido. Valores válidos: Pending, Processing, Shipped, Delivered, Cancelled");

            var order = await _orderService.UpdateOrderStatusAsync(id, status);
            if (order == null)
                return NotFound($"Pedido com ID {id} não encontrado");

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido {OrderId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? User.Identity?.Name 
            ?? "unknown";
    }
}
