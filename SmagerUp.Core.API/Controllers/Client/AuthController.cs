using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs.Client;
using SmagerUp.Core.API.Models.Client;
using SmagerUp.Core.API.Services;
using System.Security.Claims;

namespace SmagerUp.Core.API.Controllers.Client
{
    [ApiController]
    [Route("api/client")]
    public class AuthController : SucController
    {
        private readonly UserRepository _users;
        private readonly ClientRepository _clients;
        private readonly TokenService _tokenService;
        private readonly HostRepository _host;
        private readonly IPasswordService _passwordService;

        public AuthController(UserRepository Users, ClientRepository clients, TokenService tokenService, HostRepository host, IPasswordService passwordService)
        {
            _users = Users;
            _clients = clients;
            _tokenService = tokenService;
            _host = host;
            _passwordService = passwordService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> GetClientUser([FromBody] ClientUserValidateRequestDto p)
        {
            User user;
            bool isHostUser = false;
            string invalidCredentialsMsg = "Invalid client ID, API key, username, or password.";
             

            var client = await _clients.GetClientByIdAsync(p.ClientId);
            if (client == null)
            { // Host authentication
                var host = await _host.GetHostInfo();
                if (host?.HostId != p.ClientId || host?.HostKey != p.ApiKey ) return this.Fail(invalidCredentialsMsg);

                client = new Models.Core.Client
                {
                    ApiKey = p.ApiKey,
                    ClientId = p.ClientId,
                    FirstName = "Host",
                    LastName = "User"

                };

                var hostUser = await _host.GetHostUser(p.UserName);
                if (hostUser == null || !_passwordService.VerifyPassword(hostUser.PasswordHash,p.Password) ) return this.Fail(invalidCredentialsMsg);
                user = hostUser;
                isHostUser = true;
            }
            else
            { // Regular client user authentication
                if (client.ApiKey != p.ApiKey) return this.Fail(invalidCredentialsMsg);

                var clientUser = await _users.GetClientUserAsync(
                    p.ClientId,
                    p.UserName);

                if (clientUser == null || !_passwordService.VerifyPassword(clientUser.PasswordHash, p.Password))    return this.Fail(invalidCredentialsMsg);

                user = clientUser;
            }

            TokenResponseDto tokens = _tokenService.GenerateTokens(isHostUser,client, user,1,7);

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

            Guid clientId = Guid.Parse(principal.FindFirst("ClientId")!.Value);
            Guid userId = Guid.Parse(principal.FindFirst("UserId")!.Value);
            bool isHostUser = bool.Parse(principal.FindFirst("IsHostUser")!.Value);

           
            var client = await _clients.GetClientByIdAsync(clientId);

            var user = await _users.GetUserByIdAsync(clientId, userId);

            if (client == null || user == null)
                return this.Fail("Invalid refresh token.");

            var accessToken = _tokenService.GenerateAccessToken(isHostUser, client,user,1);


            return this.Success(
                new
                {
                    AccessToken = accessToken
                },
                "Token refreshed successfully.");
        }
    }
}
