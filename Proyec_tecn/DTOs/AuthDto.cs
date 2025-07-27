namespace Proyec_tecn.DTOs
{
   
        public class LoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class TokenDto
        {
            public string Token { get; set; } = string.Empty;
            public DateTime Expiration { get; set; }
            public string Username { get; set; } = string.Empty;
        }
    }

