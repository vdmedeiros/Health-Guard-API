using HealthcareApi.Application.DTOs;

namespace HealthcareApi.Application.Services;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    string GenerateToken(UserDto user);
}
