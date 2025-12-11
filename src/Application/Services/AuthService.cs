using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using HealthcareApi.Application.DTOs;
using HealthcareApi.Domain.Entities;
using HealthcareApi.Domain.Interfaces;

namespace HealthcareApi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var userDto = new UserDto(user.Id, user.Email, user.Name, user.Role, user.IsActive);
        var token = GenerateToken(userDto);
        var expiresAt = DateTime.UtcNow.AddHours(GetTokenExpirationHours());

        return new AuthResponse(token, user.Email, user.Name, user.Role, expiresAt);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return null;

        var user = new User
        {
            Email = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name.Trim(),
            Role = "User"
        };

        await _userRepository.AddAsync(user);

        var userDto = new UserDto(user.Id, user.Email, user.Name, user.Role, user.IsActive);
        var token = GenerateToken(userDto);
        var expiresAt = DateTime.UtcNow.AddHours(GetTokenExpirationHours());

        return new AuthResponse(token, user.Email, user.Name, user.Role, expiresAt);
    }

    public string GenerateToken(UserDto user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "HealthcareApi",
            audience: _configuration["Jwt:Audience"] ?? "HealthcareApi",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(GetTokenExpirationHours()),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GetJwtKey()
    {
        return _configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultSecretKey12345678901234567890";
    }

    private int GetTokenExpirationHours()
    {
        var hours = _configuration["Jwt:ExpirationHours"];
        return int.TryParse(hours, out var result) ? result : 24;
    }
}
