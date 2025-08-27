using Microsoft.AspNetCore.Mvc;
using ApiGateway.Services;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username e password são obrigatórios");
            }

            var token = await _authService.LoginAsync(request.Username, request.Password);
            
            if (token == null)
            {
                return Unauthorized("Credenciais inválidas");
            }

            return Ok(new { Token = token, Message = "Login realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.RegisterAsync(request);
            
            if (user == null)
            {
                return BadRequest("Usuário ou email já existe");
            }

            return Ok(new { 
                Message = "Usuário registrado com sucesso",
                UserId = user.Id,
                Username = user.Username 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o registro");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("me")]
    public async Task<ActionResult> GetCurrentUser()
    {
        try
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário atual");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}
