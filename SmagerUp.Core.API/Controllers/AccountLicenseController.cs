using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers
{
    [Authorize]
    [Route("api/license")]
    public class AccountLicenseController : SucController
    {
        private readonly AccountLicenseRepository _accountLicense;

        public AccountLicenseController(AccountLicenseRepository accountLicense)
        {
            _accountLicense = accountLicense;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetLicenses()
        {
            if (this.AccountId is null)
                return Unauthorized(new { error = "Missing account information in token." });

            var list = await _accountLicense.GetLicensesForAccount(this.AccountId.Value);
            return Ok(new
            {
                AccountName = this.AccountName,
                AccountLicenses = list
            });
        }


    }
}
