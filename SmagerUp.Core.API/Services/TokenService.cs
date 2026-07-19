using Microsoft.IdentityModel.Tokens;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Models;
using SmagerUp.Core.API.Models.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmagerUp.Core.API.Services;

public class TokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }


    public TokenResponseDto GenerateTokens(string ApiKey, User user, int accessTokenExpires, int refreshTokenExpires )
    {
        var accessToken = GenerateAccessToken(ApiKey, user, accessTokenExpires);
        var refreshToken = GenerateRefreshToken(ApiKey,user, refreshTokenExpires);

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


    public string GenerateAccessToken(string ApiKey, User user, int accessTokenExpires)
    {
        var claims = new[]
        {
            new Claim("TokenType", "Access"),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("UserName", user.UserName),
            new Claim("ApiKey", ApiKey),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
        };

        return GenerateJwtToken(claims,DateTime.UtcNow.AddMinutes(accessTokenExpires));
    }

    public string GenerateRefreshToken(string ApiKey, User user, int refreshTokenExpires)
    {
        var claims = new[]
        {
            new Claim("TokenType", "Refresh"),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("UserName", user.UserName),
            new Claim("ApiKey", ApiKey),
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