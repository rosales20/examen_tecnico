using Microsoft.AspNetCore.Mvc;
using Proyec_tecn.DTOs;
using Proyec_tecn.Services;

namespace Proyec_tecn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login(LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);
            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(token);
        }
    }
}
