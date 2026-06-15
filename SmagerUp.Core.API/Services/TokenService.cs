using Microsoft.IdentityModel.Tokens;
using SmagerUp.Core.API.DTOs.Client;
using SmagerUp.Core.API.Models.Client;
using SmagerUp.Core.API.Models.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmagerUp.Core.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }


        public TokenResponseDto GenerateTokens(Client client, User user, int accessTokenExpires, int refreshTokenExpires )
        {
            var accessToken = GenerateAccessToken(client, user, accessTokenExpires);
            var refreshToken = GenerateRefreshToken(client, user, refreshTokenExpires);

            return (
                    new TokenResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessTokenExpires = DateTime.UtcNow.AddMinutes(accessTokenExpires),
                        RefreshTokenExpires=DateTime.UtcNow.AddDays(refreshTokenExpires)  

                    }
            );
            
        }


        public string GenerateAccessToken(Client client, User user, int accessTokenExpires)
        {
            var claims = new[]
            {
                new Claim("TokenType", "Access"),
                new Claim("ClientId", client.ClientId.ToString()),
                new Claim("ApiKey", client.ApiKey.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserName", user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            return GenerateJwtToken(claims,DateTime.UtcNow.AddMinutes(accessTokenExpires));
        }

        public string GenerateRefreshToken(Client client, User user, int refreshTokenExpires)
        {
            var claims = new[]
            {
                new Claim("TokenType", "Refresh"),
                new Claim("ClientId", client.ClientId.ToString()),
                new Claim("ApiKey", client.ApiKey.ToString()),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("UserName", user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            return GenerateJwtToken(claims,DateTime.UtcNow.AddDays(refreshTokenExpires));
        }

        private string GenerateJwtToken(Claim[] claims,DateTime expires)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters =new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidAudience = _config["Jwt:Audience"],
                        IssuerSigningKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };

                var principal = tokenHandler.ValidateToken(token,validationParameters,out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}