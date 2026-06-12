using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmagerUp.Core.API.Controllers
{
    [Authorize]
    [Route("api/files")]
    public class FileController : SucController
    {
        [HttpGet("info")]
        public IActionResult Info()
        {
            return Ok(new { account = ClientName, message = "You’re authorized to access files." });
        }
    }

}
