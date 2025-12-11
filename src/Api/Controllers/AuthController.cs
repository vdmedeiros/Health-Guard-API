using Microsoft.AspNetCore.Mvc;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Application.Services;

namespace HealthcareApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email e senha são obrigatórios." });

        var result = await _authService.LoginAsync(request);
        
        if (result == null)
            return Unauthorized(new { message = "Email ou senha inválidos." });

        return Ok(result);
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Email, senha e nome são obrigatórios." });

        if (request.Password.Length < 6)
            return BadRequest(new { message = "A senha deve ter pelo menos 6 caracteres." });

        var result = await _authService.RegisterAsync(request);
        
        if (result == null)
            return BadRequest(new { message = "Este email já está cadastrado." });

        return CreatedAtAction(nameof(Login), result);
    }
}
