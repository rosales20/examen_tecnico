using Proyec_tecn.DTOs;

namespace Proyec_tecn.Services
{
    public interface IAuthService
    {
        Task<TokenDto?> LoginAsync(LoginDto loginDto);
        string GenerateJwtToken(string username);
    }
}
