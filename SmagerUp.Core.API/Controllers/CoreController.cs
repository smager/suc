using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.DTOs;

namespace SmagerUp.Core.API.Controllers;

[ApiController]
[Route("api")]
public class CoreController : SucController
{
    private readonly AccountRepository _accounts;
    private readonly ModuleRepository _modules;
    public CoreController(AccountRepository a, ModuleRepository m)
    {
        _accounts = a;
        _modules = m;
    }

    [HttpPost("validate-account")]
    public async Task<IActionResult> ValidateAccount([FromBody] ValidateRequestDto dto)
    {
        var acc = await _accounts.ValidateAsync(dto.AccountId, dto.Key);
        if (acc == null) return Unauthorized(new { error = "Invalid account or key" });

        var mods = await _modules.GetModulesForAccount(dto.AccountId);
        return Ok(new { account = $"{acc.FirstName} {acc.LastName}", modules = mods.Select(x => new { x.Name, x.Version, x.Price }) });
    }

    [Authorize]
    [HttpGet("{name}@{version}")]
    public async Task<IActionResult> GetModule(string name, string version)
    {
        if (this.AccountId is null)
            return Unauthorized(new { error = "Missing account information in token." });

        // Safely parse the GUID
        if (!Guid.TryParse(this.AccountId.ToString(), out var accountId))
            return BadRequest(new { error = "Invalid account ID format in token." });

        // Load the requested module
        var mod = await _modules.GetModuleAsync(name, version);
        if (mod == null)
            return NotFound(new { error = "Module not found" });

        // Optional access control
        var allowed = (await _modules.GetModulesForAccount(accountId))
            .Any(m => m.Name == name);

        if (!allowed)
            return Forbid();

        // 🔹 Return response
        return Ok(new
        {
            name = mod.Name,
            version = mod.Version,
            code = mod.Code,
            account = this.AccountName
        });
    }


}
