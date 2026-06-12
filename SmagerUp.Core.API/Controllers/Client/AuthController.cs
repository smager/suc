using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs.Client;
using SmagerUp.Core.API.DTOs.Core;
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

        // [Authorize]
        [HttpPost("login")]
        public async Task<IActionResult> GetClientUser([FromBody] ClientUserValidateRequestDto p)
        {



            var client = await _clients.ValidateAsync(p.ClientId, p.Key);
            if (client == null)
                return this.Fail("Invalid client ID or API key.");


            var user = await _users.ValidateClientUserAsync(this.ClientId, p.UserName, p.Password);
            if (user == null)
                return this.Fail("Invalid Username or Password.");


            var token = _tokenService.GenerateToken(client, user);


            return this.Success(
                new {
                    Token = token
                    ,user.UserId
                    ,user.UserName
                    ,user.FirstName
                    ,user.LastName
                }
                , "User successfully Logged In."
            );
        }
    }
}
