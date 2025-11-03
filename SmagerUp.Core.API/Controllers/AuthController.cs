using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Extensions;
using SmagerUp.Core.API.Services;

namespace SmagerUp.Core.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AccountRepository _accounts;
        private readonly TokenService _tokenService;

        public AuthController(AccountRepository accounts, TokenService tokenService)
        {
            _accounts = accounts;
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] ValidateRequestDto dto)
        {
            var acc = await _accounts.ValidateAsync(dto.AccountId, dto.Key);
            if (acc == null)
                return this.Fail("Invalid account ID or API key.");

            var token = _tokenService.GenerateToken(acc);
            return this.Success(new { token, account = $"{acc.FirstName} {acc.LastName}" }, "Token generated successfully.");
        }
    }
}
