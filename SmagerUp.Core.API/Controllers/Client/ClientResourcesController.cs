using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.Models.Client;

namespace SmagerUp.Core.API.Controllers.Client;

[ApiController]
[Route("api/client/resources")]
public class ClientResourcesController : SucController
{
    private readonly ClientRepository _repo;

    public ClientResourcesController(ClientRepository repo)
    {
        _repo = repo;
    }

    // GET /api/client/resources
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        var list = await _repo.GetAllResourcesAsync(ClientId);
        return this.Success(list);
    }

    // GET /api/client/resources/{name}
    // returns available versions for a resource name
    [Authorize]
    [HttpGet("{name}")]
    public async Task<IActionResult> GetVersionsByName(string name)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        var result = await _repo.GetVersionsByNameAsync(ClientId, name);
        if (!result.Any())
            return this.Fail("Resource not found.");

        return this.Success(result);
    }

    // GET /api/client/resources/{name}/{version}
    [Authorize]
    [HttpGet("{name}/{version:int}")]
    public async Task<IActionResult> GetByNameAndVersion(string name, int version)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        var resource = await _repo.GetByNameAndVersionAsync(ClientId, name, version);
        if (resource is null)
            return this.Fail("Resource not found.");

        return this.Success(resource);
    }

    // POST /api/client/resources
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Resource model)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        if (string.IsNullOrWhiteSpace(model.ResourceName))
            return this.Fail("ResourceName is required.");

        if (string.IsNullOrWhiteSpace(model.ResourceType))
            return this.Fail("ResourceType is required.");

        if (model.Version is null)
            return this.Fail("Version is required.");

        var id = model.ResourceId == Guid.Empty ? Guid.NewGuid() : model.ResourceId;
        model.ResourceId = id;
        model.IsDeleted = false;
        model.IsActive = true;
        model.CreatedAt = DateTime.UtcNow;
        model.CreatedBy = UserId;

        var rows = await _repo.CreateResourceAsync(ClientId, model);
        if (rows == 0)
            return this.Fail("Failed to create resource.");

        return this.Success(new { resourceId = id });
    }

    // PUT /api/client/resources
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] Resource model)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        if (model.ResourceId == Guid.Empty)
            return this.Fail("ResourceId is required for update.");

        var exists = await _repo.ResourceExistsAsync(ClientId, model.ResourceId);
        if (!exists)
            return this.Fail("Resource not found.");

        model.UpdatedAt = DateTime.UtcNow;
        model.UpdatedBy = UserId;

        var rows = await _repo.UpdateResourceAsync(ClientId, model);
        if (rows == 0)
            return this.Fail("Failed to update resource.");

        return this.Success(new { resourceId = model.ResourceId });
    }

    // PATCH /api/client/resources/{resourceId}/active
    // body: { "isActive": true }
    [Authorize]
    [HttpPatch("{resourceId:guid}/active")]
    public async Task<IActionResult> SetActive(Guid resourceId, [FromBody] SetActiveDto dto)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        if (resourceId == Guid.Empty)
            return this.Fail("Invalid resource identifier.");

        if (dto is null)
            return this.Fail("Missing payload.");

        var exists = await _repo.ResourceExistsAsync(ClientId, resourceId);
        if (!exists)
            return this.Fail("Resource not found or deleted.");

        var now = DateTime.UtcNow;
        var rows = await _repo.SetActiveByIdAsync(ClientId, resourceId, dto.IsActive, UserId, now);
        if (rows == 0)
            return this.Fail("Failed to update resource active state.");

        return this.Success(new { resourceId, isActive = dto.IsActive });
    }

    // Optional convenience: set active by name+version
    [Authorize]
    [HttpPatch("{name}/{version:int}/active")]
    public async Task<IActionResult> SetActiveByNameVersion(string name, int version, [FromBody] SetActiveDto dto)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        if (dto is null)
            return this.Fail("Missing payload.");

        var now = DateTime.UtcNow;
        var rows = await _repo.SetActiveByNameVersionAsync(ClientId, name, version, dto.IsActive, UserId, now);
        if (rows == 0)
            return this.Fail("Resource not found or failed to update.");

        var resourceId = await _repo.FindResourceIdByNameVersionAsync(ClientId, name, version);
        return this.Success(new { resourceId, isActive = dto.IsActive });
    }

    // DELETE /api/client/resources/{resourceId}
    // Soft delete: sets IsDeleted = 1 and updates DeletedAt/DeletedBy and IsActive = 0
    [Authorize]
    [HttpDelete("{resourceId:guid}")]
    public async Task<IActionResult> Delete(Guid resourceId)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        if (resourceId == Guid.Empty)
            return this.Fail("Invalid resource identifier.");

        var exists = await _repo.ResourceExistsAsync(ClientId, resourceId);
        if (!exists)
            return this.Fail("Resource not found or already deleted.");

        var deletedAt = DateTime.UtcNow;
        var rows = await _repo.SoftDeleteByIdAsync(ClientId, resourceId, UserId, deletedAt);
        if (rows == 0)
            return this.Fail("Failed to delete resource.");

        return this.Success(new { resourceId });
    }

    // Optional convenience: DELETE by name and version
    [Authorize]
    [HttpDelete("{name}/{version:int}")]
    public async Task<IActionResult> DeleteByNameVersion(string name, int version)
    {
        if (ClientId == Guid.Empty)
            return this.Fail("Missing account information in token.");

        var deletedAt = DateTime.UtcNow;
        var rows = await _repo.SoftDeleteByNameVersionAsync(ClientId, name, version, UserId, deletedAt);
        if (rows == 0)
            return this.Fail("Resource not found or already deleted.");

        var resourceId = await _repo.FindResourceIdByNameVersionAsync(ClientId, name, version);
        return this.Success(new { resourceId });
    }

    public class SetActiveDto
    {
        public bool IsActive { get; set; }
    }
}
