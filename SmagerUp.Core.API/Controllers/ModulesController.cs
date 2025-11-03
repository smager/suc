using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Extensions;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers;

[ApiController]
[Route("api/modules")]
public class ModulesController : SucController
{
    private readonly ModuleRepository _modules;
    private readonly ContentRepository _contents;
    private readonly LicenseTypeRepository _licenses;

    public ModulesController(ModuleRepository modules, ContentRepository contents, LicenseTypeRepository licenses)
    {
        _modules = modules;
        _contents = contents;
        _licenses = licenses;
    }

    [Authorize]
    [HttpGet("{name}@{version}")]
    public async Task<IActionResult> GetModule(string name, string version)
    {
        // 🔹 Verify account identity
        if (AccountId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        // 🔹 Fetch module
        var mod = await _modules.GetModuleAsync(name, version);
        if (mod == null)
            return this.Fail("Module not found.");

        // 🔹 Validate access
        var allowedModules = await _modules.GetModulesForAccountAsync(AccountId);
        var allowed = allowedModules.Any(m => m.Name == name && m.Version == version);

        // 🔹 Get license name
        var licenseName = await _licenses.GetLicenseNameAsync(mod.LicenseTypeId) ?? string.Empty;

        // 🔹 Load associated contents
        var contents = Enumerable.Empty<ModuleContentDto>();
        if (mod.ContentGroupId.HasValue)
        {
            var contentRows = await _contents.GetContentsByGroupAsync(mod.ContentGroupId.Value);
            contents = contentRows.Select(c => new ModuleContentDto
            {
                Type = c.ContentType,
                Body = c.ContentBody
            });
        }

        // 🔹 Package module info
        var moduleInfo = new
        {
            name = mod.Name,
            version = mod.Version,
            license = licenseName,
            price = mod.Price,
            contents
        };

        // 🔹 Free or owned module
        if (mod.LicenseTypeId == 1 || allowed)
            return this.Success(moduleInfo);

        // 🔹 Paid and not owned
        return this.Fail("Need to purchase or your license has expired.", moduleInfo);
    }


}
