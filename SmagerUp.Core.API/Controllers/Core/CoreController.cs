using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Services;
using System.Text.Json;

namespace SmagerUp.Core.API.Controllers.Core{

    [ApiController]
    [Route("core")]
    public class CoreController : SucController
    {
        private readonly IEncryptionService _encryption;
        private readonly ICoreDataRepository _data;
        private readonly IPasswordService _passwordService;

        public CoreController(ICoreDataRepository data, IEncryptionService encryption, IPasswordService passwordService)
        {
            _encryption = encryption;
            _data = data;
            _passwordService = passwordService;
        }
 
    
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] DataRequest request)
        {
            var password =((JsonElement)request.Parameters["PasswordHash"]).GetString();

            request.Parameters["PasswordHash"] = _passwordService.HashPassword(password);
            var result =    await _data.ExecuteAsync(this.UserId ,request);
            return Ok(result);

        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid token)
        {
            DataRequest request = new DataRequest
            {
                ActionCode = "US-4F17-D671",
                Parameters = new Dictionary<string, object>
            {
                { "tokenId", token }
            }
            };

            dynamic res = await _data.ExecuteAsync(this.UserId, request);

            if (!res.isSuccess)
                return BadRequest(res);

            string redirectUrl = res.result.redirectUrl;

            return Redirect(redirectUrl);
        }


    } 
}