using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.DTOs.Client;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Controllers;

[ApiController]
[Route("client/auth")]
public class AuthController : SucController
{
    private readonly ClientRepository _clientRepository;
    private readonly Data.Admin.AdminRepository _adminRepository;
    private readonly TokenService _tokenService;
    private readonly IPasswordService _passwordService;
    const int _refreshTokenExpire= 7; // days
    const int _accessTokenExpire = 30; // mins


    public AuthController(ClientRepository clientRepository, Data.Admin.AdminRepository adminRepository, TokenService tokenService, IPasswordService passwordService)
    {
        _clientRepository = clientRepository;
        _adminRepository = adminRepository;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> GetClientUser([FromBody] AppValidateRequestDto p)
    {
        string invalidCredentialsMsg = "Invalid client ID, API key, username, or password.";
        var app = await  _adminRepository.GetAppByApiKeyAsync(p.ApiKey);

        if (app.ApiKey != p.ApiKey) return this.Fail(invalidCredentialsMsg);
        
        var user = await _clientRepository.GetUserByUserNameAsync(app.ApiKey,p.UserName);
        if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, p.Password))    return this.Fail(invalidCredentialsMsg);
    
        TokenResponseDto tokens = _tokenService.GenerateTokens(app.ApiKey, user, _accessTokenExpire, _refreshTokenExpire);

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

        string ApiKey = principal.FindFirst("ApiKey")!.Value;
        Guid userId = Guid.Parse(principal.FindFirst("UserId")!.Value);

        var user = await _clientRepository.GetUserByIdAsync(ApiKey, userId);

        if ( user == null)
            return this.Fail("Invalid refresh token.");

        var accessToken = _tokenService.GenerateAccessToken(ApiKey,user, _accessTokenExpire);


        return this.Success(
            new
            {
                AccessToken = accessToken
            },
            "Token refreshed successfully.");
    }
}
