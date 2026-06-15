using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs.Client;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Controllers.Client
{
    [ApiController]
    [Route("api/client")]
    public class AuthController : SucController
    {
        private readonly UserRepository _users;
        private readonly ClientRepository _clients;
        private readonly TokenService _tokenService;

        public AuthController(UserRepository Users, ClientRepository clients, TokenService tokenService)
        {
            _users = Users;
            _clients = clients;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetClientUser([FromBody] ClientUserValidateRequestDto p)
        {


            var client = await _clients.ValidateClientAsync(p.ClientId, p.Key);
            if (client == null)
                return this.Fail("Invalid client ID or API key.");


            var user = await _users.ValidateClientUserAsync(p.ClientId, p.UserName, p.Password);
            if (user == null)
                return this.Fail("Invalid Username or Password.");

            TokenResponseDto tokens = _tokenService.GenerateTokens(client, user,1,7);

     
            return this.Success(
                new
                {
                    tokens,
                    user.UserId,
                    user.UserName,
                },
                "User successfully Logged In."
            );
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken( [FromBody] RefreshTokenRequestDto p)
        {
            var principal = _tokenService.ValidateToken(p.RefreshToken);

            if (principal == null)
                return this.Fail("Invalid refresh token.");

            var tokenType = principal.FindFirst("TokenType")?.Value;

            if (tokenType != "Refresh") return this.Fail("Invalid refresh token.");

            Guid clientId = Guid.Parse(principal.FindFirst("ClientId")!.Value);

            Guid userId = Guid.Parse(principal.FindFirst("UserId")!.Value);

            var client = await _clients.GetClientByIdAsync(clientId);

            var user = await _users.GetUserByIdAsync(clientId, userId);

            if (client == null || user == null)
                return this.Fail("Invalid refresh token.");

            var accessToken = _tokenService.GenerateAccessToken(client,user,1);


            return this.Success(
                new
                {
                    AccessToken = accessToken
                },
                "Token refreshed successfully.");
        }
    }
}
