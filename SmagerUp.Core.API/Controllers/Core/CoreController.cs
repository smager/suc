using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Services;
using System.Text.Json;

namespace SmagerUp.Core.API.Controllers.Core;

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
        var result =    await _data.ExecuteAsync(Guid.Empty ,request);
        return Ok(result);

    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] JsonElement body)
    {
        var token = body.GetProperty("token").GetString();
        return Ok(await _data.ExecuteAsync(Guid.Empty, new DataRequest
        {
            ActionCode = "US-4F17-D671",
            Parameters = new()
            {
                ["tokenId"] = token
            }
        }));
    }


} 
 