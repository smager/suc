using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Controllers;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;

[ApiController]
[Route("client/data")]
[Authorize]
public class DataController : SucController
{
    private readonly IClientDataRepository _data;

    public DataController(IClientDataRepository data){
        _data = data;
    }

    [HttpPost("execute")]
    public async Task<IActionResult> Execute([FromBody] DataRequest request){
        return Ok( await _data.ExecuteAsync(ClientId,UserId,request));
    }
 
}