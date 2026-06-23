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
    private readonly IDataRepository _data;

    public DataController(
        IDataRepository data)
    {
        _data = data;
    }

    [HttpPost("getdata")]
    public async Task<IActionResult> GetData(
        [FromBody] DataRequest request)
    {
        return Ok(
            await _data.GetDataAsync(
                ClientId,
                UserId,
                request));
    }

    [HttpPost("executecmd")]
    public async Task<IActionResult> ExecuteCmd(
        [FromBody] DataRequest request)
    {
        return Ok(
            await _data.ExecuteCmdAsync(
                ClientId,
                UserId,
                request));
    }
}