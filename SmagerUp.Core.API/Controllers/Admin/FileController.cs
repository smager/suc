using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmagerUp.Core.API.Controllers.Core;

[Authorize]
[Route("admin/files")]
public class FileController : SucController
{
    [HttpGet("info")]
    public IActionResult Info()
    {
        return Ok(new { UserName, message = "You’re authorized to access files." });
    }
}
