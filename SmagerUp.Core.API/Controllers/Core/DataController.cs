using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Controllers.Core{
    [ApiController]
    [Route("core/data")]
    [Authorize]
    public class DataController : SucController
    {
        private readonly ICoreDataRepository _data;

        public DataController(ICoreDataRepository data){
            _data = data;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> Execute([FromBody] DataRequest request){
            return Ok( await _data.ExecuteAsync(UserId,request));
        }
 
    } 
}