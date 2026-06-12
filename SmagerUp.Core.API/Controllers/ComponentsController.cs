using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Extensions;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Controllers;

[ApiController]
[Route("api/components")]
public class ComponentsController : SucController
{
    private readonly ComponentRepository _components;
    private readonly ResourceRepository _resources;
    private readonly LicenseTypeRepository _licenses;

    public ComponentsController(ComponentRepository components, ResourceRepository resources, LicenseTypeRepository licenses)
    {
        _components = components;
        _resources = resources;
        _licenses = licenses;
    }

    // GET /api/components/{name}@{version}
    // Accepts version as string in route, validates it is an integer before querying DB
    [Authorize]
    [HttpGet("{name}@{version}")]
    public async Task<IActionResult> GetComponent(string name, string version)
    {
        

        var comp = await _components.GetComponentAsync(name, version);
        if (comp == null)
            return this.Fail("Component not found.");

       

        var resourcesDto = Enumerable.Empty<ResourceDto>();
        var resources = await _resources.GetResourcesByComponentAsync(comp.ComponentId);
        if (resources != null && resources.Any())
        {
            resourcesDto = resources.Select(r => new ResourceDto
            {
                Type = r.ResourceType,
                Body = r.ResourceBody
            });
        }

        var componentInfo = new ComponentResponseDto
        {
            Name = comp.Name,
            Version = comp.Version,
          
            Price = comp.Price,
            Resources = resourcesDto
        };

        return this.Success(componentInfo);
    }

    // GET /api/components
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetComponents()
    {
        var components = await _components.GetAllAsync();

        var list = components.Select(c => new
        {
            name = c.Name,
            version = c.Version,
            price = c.Price,
            licenseTypeId = c.LicenseTypeId
        });

        return this.Success(list);
    }

    // GET /api/components/client
    [Authorize]
    [HttpGet("client")]
    public async Task<IActionResult> GetComponentsByClient()
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        var components = await _components.GetClientComponentsAsync(ClientId);
        var list = components.Select(c => new
        {
            name = c.Name,
            version = c.Version,
            price = c.Price,
            licenseTypeId = c.LicenseTypeId
        });

        return this.Success(list);
    }
}