using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Controllers.Client
{
    [ApiController]
    [Route("api/client/login")]
    public class AuthController : ControllerBase
    {
        private readonly ClientRepository _clients;
        private readonly TokenService _tokenService;

        public AuthController(ClientRepository clients, TokenService tokenService)
        {
            _clients = clients;
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] ValidateRequestDto dto)
        {
            var client = await _clients.ValidateAsync(dto.ClientId, dto.Key);
            if (client == null)
                return this.Fail("Invalid client ID or API key.");

            var token = _tokenService.GenerateToken(client);
            return this.Success(new { token, Client = $"{client.FirstName} {client.LastName}" }, "Token generated successfully.");
        }
    }
}
