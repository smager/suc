using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Controllers;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Services;

[ApiController]
[Route("app")]
[Authorize]
public class AppController : SucController
{
    private readonly IEncryptionService _encryption;


    public AppController(IClientDataRepository data, IEncryptionService encryption)
    {
        _encryption = encryption;
    }
 
    [Authorize]
    [HttpPost("encrypt")]
    public IActionResult Encrypt(
        [FromBody] string body)
    {
        return Ok(_encryption.Encrypt(body));
    }

    [Authorize]
    [HttpPost("decrypt")]
    public IActionResult Decrypt(
        [FromBody] string body)
    {
        return Ok(_encryption.Decrypt(body));
    }

}