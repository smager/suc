using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.Extensions;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers
{
    [Authorize]
    [Route("api/licenseTypes")]
    public class LicenseTypesController : SucController
    {
        private readonly LicenseTypeRepository _licenseTypeRepository;

        public LicenseTypesController(LicenseTypeRepository licenseTypeRepository)
        {
            _licenseTypeRepository = licenseTypeRepository;
        }

        [Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetLicenses()
        {
            if (this.AccountId is null)
                return this.Fail("Missing account information in token.");

            var list = await _licenseTypeRepository.GetAllAsync();
           
            return this.Success(list);
        }


    }
}
