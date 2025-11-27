using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Models;

namespace TaskManagement.Services
{
    
    public interface ITokenService
    {
        string GenerateToken(User user);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                // User's ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                // User's email
                new Claim(ClaimTypes.Email, user.Email),
                // User's name
                new Claim(ClaimTypes.Name, user.FullName),
                // Unique token ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Get the secret key from configuration
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)
            );

            // Create signing credentials using the secret key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["JwtSettings:ExpirationInMinutes"]!)
                ),
                signingCredentials: credentials
            );

            // Convert the token to a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
