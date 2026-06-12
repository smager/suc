using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SmagerUp.Core.API.Models.Client;
using SmagerUp.Core.API.Models.Core;

namespace SmagerUp.Core.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Client client, User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                // Client
                new Claim("ClientId", client.ClientId.ToString()),

                // User
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserName", user.UserName),

                // Standard Claims
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),

                // Display Name
                new Claim(
                JwtRegisteredClaimNames.UniqueName,
                $"{user.FirstName} {user.LastName}"),

                new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
