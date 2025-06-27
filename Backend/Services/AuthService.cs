using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project8.Backend.Models;

namespace Project8.Backend.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(User user)
        {

            var userId = user?.Id ?? string.Empty;
            var userName = user?.UserName ?? string.Empty;
            var email = user?.Email ?? string.Empty;
            var role = user?.Role ?? "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var keyString = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireDays = double.TryParse(_configuration["Jwt:ExpireDays"], out var days) ? days : 7;
            var expires = DateTime.UtcNow.AddDays(expireDays);

            var issuer = _configuration["Jwt:Issuer"] ?? "defaultIssuer";
            var audience = _configuration["Jwt:Audience"] ?? "defaultAudience";

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public string DetermineRole(string email)
        {
            return email.EndsWith(".admin@examapp.com", StringComparison.OrdinalIgnoreCase) ? "Admin" : "User";
        }
    }
}
