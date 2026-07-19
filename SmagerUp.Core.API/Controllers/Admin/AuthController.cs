using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Admin;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.DTOs.Admin;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Controllers.Admin;

[ApiController]
[Route("admin/auth")]
public class AuthController : SucController
{
    private readonly AdminRepository _admin;
    private readonly TokenService _tokenService;
    private readonly IPasswordService _passwordService;
    const int _refreshTokenExpire= 7; // days
    const int _accessTokenExpire = 30; // mins


    public AuthController(AdminRepository admin, TokenService tokenService, IPasswordService passwordService)
    {
        _admin = admin;
        _tokenService = tokenService;
       
        _passwordService = passwordService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GetClientUser([FromBody] AdminUserValidateRequestDto p)
    {      
        string invalidCredentialsMsg = "Invalid API key, username, or password.";         
        //if (client.ApiKey != p.ApiKey) return this.Fail(invalidCredentialsMsg);

        var user = await _admin.GetUser(p.UserName);
        if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, p.Password))    return this.Fail(invalidCredentialsMsg);
        TokenResponseDto tokens = _tokenService.GenerateTokens( string.Empty, user, _accessTokenExpire, _refreshTokenExpire);

        return this.Success(
            new
            {
                tokens,
                user.UserId,
                user.UserName,
            },
            "User successfully Logged In.");
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken( [FromBody] RefreshTokenRequestDto p)
    {
        var principal = _tokenService.ValidateToken(p.RefreshToken);
        if (principal == null)
            return this.Fail("Invalid refresh token.");

        var tokenType = principal.FindFirst("TokenType")?.Value;

        if (tokenType != "Refresh") return this.Fail("Invalid refresh token.");

        Guid userId = Guid.Parse(principal.FindFirst("UserId")!.Value);

        var user = await _admin.GetUserById(userId);

        if (user == null)
            return this.Fail("Invalid refresh token.");

        var accessToken = _tokenService.GenerateAccessToken(string.Empty, user, _accessTokenExpire);


        return this.Success(
            new
            {
                AccessToken = accessToken
            },
            "Token refreshed successfully.");
    }
}
