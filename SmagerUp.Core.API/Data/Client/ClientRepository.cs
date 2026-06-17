using Dapper;
using SmagerUp.Core.API.Models.Client;
using System.Data;

namespace SmagerUp.Core.API.Data.Client;

public class ClientRepository
{
    private readonly IClientDbResolver _resolver;

    public ClientRepository(IClientDbResolver resolver) => _resolver = resolver;

    public async Task<IEnumerable<Resource>> GetAllResourcesAsync(Guid clientId)
    {
        const string sql = @"
            SELECT * FROM Resources
            WHERE IsDeleted = 0
            ORDER BY ResourceName, Version;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.QueryAsync<Resource>(sql);
    }

    public async Task<IEnumerable<Resource>> GetVersionsByNameAsync(Guid clientId, string name)
    {
        const string sql = @"
            SELECT * FROM Resources
            WHERE IsDeleted = 0 AND ResourceName = @name
            ORDER BY Version DESC;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.QueryAsync<Resource>(sql, new { name });
    }

    public async Task<Resource?> GetByNameAndVersionAsync(Guid clientId, string name, int version)
    {
        const string sql = @"
            SELECT  * FROM Resources
            WHERE IsDeleted = 0 AND ResourceName = @name AND Version = @version;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.QueryFirstOrDefaultAsync<Resource>(sql, new { name, version });
    }

    public async Task<int> CreateResourceAsync(Guid clientId, Resource model)
    {
        const string sql = @"
            INSERT INTO Resources
                (ResourceId, ResourceName, ResourceType, ResourceBody, Version, IsDeleted, IsActive, CreatedAt, CreatedBy)
            VALUES
                (@ResourceId, @ResourceName, @ResourceType, @ResourceBody, @Version, @IsDeleted, @IsActive, @CreatedAt, @CreatedBy);";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.ExecuteAsync(sql, model);
    }

    public async Task<int> UpdateResourceAsync(Guid clientId, Resource model)
    {
        const string sql = @"
            UPDATE Resources
            SET ResourceName = @ResourceName,
                ResourceType = @ResourceType,
                ResourceBody = @ResourceBody,
                Version = @Version,
                UpdatedAt = @UpdatedAt,
                UpdatedBy = @UpdatedBy
            WHERE ResourceId = @ResourceId AND IsDeleted = 0;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.ExecuteAsync(sql, model);
    }

    public async Task<bool> ResourceExistsAsync(Guid clientId, Guid resourceId)
    {
        const string sql = @"
            SELECT TOP 1 ResourceId
            FROM Resources
            WHERE ResourceId = @ResourceId AND IsDeleted = 0;";
        using var conn = _resolver.CreateConnection(clientId);
        var id = await conn.QueryFirstOrDefaultAsync<Guid?>(sql, new { ResourceId = resourceId });
        return id.HasValue && id.Value != Guid.Empty;
    }

    public async Task<int> SoftDeleteByIdAsync(Guid clientId, Guid resourceId, Guid deletedBy, DateTime deletedAt)
    {
        const string sql = @"
            UPDATE Resources
            SET IsDeleted = 1,
                IsActive = 0,
                DeletedAt = @DeletedAt,
                DeletedBy = @DeletedBy
            WHERE ResourceId = @ResourceId AND IsDeleted = 0;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.ExecuteAsync(sql, new { ResourceId = resourceId, DeletedAt = deletedAt, DeletedBy = deletedBy });
    }

    public async Task<Guid?> FindResourceIdByNameVersionAsync(Guid clientId, string name, int version)
    {
        const string sql = @"
            SELECT TOP 1 ResourceId
            FROM Resources
            WHERE ResourceName = @name AND Version = @version AND IsDeleted = 0;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.QueryFirstOrDefaultAsync<Guid?>(sql, new { name, version });
    }

    public async Task<int> SoftDeleteByNameVersionAsync(Guid clientId, string name, int version, Guid deletedBy, DateTime deletedAt)
    {
        var resourceId = await FindResourceIdByNameVersionAsync(clientId, name, version);
        if (resourceId is null || resourceId == Guid.Empty)
            return 0;
        return await SoftDeleteByIdAsync(clientId, resourceId.Value, deletedBy, deletedAt);
    }

    public async Task<int> SetActiveByIdAsync(Guid clientId, Guid resourceId, bool isActive, Guid updatedBy, DateTime updatedAt)
    {
        const string sql = @"
            UPDATE Resources
            SET IsActive = @IsActive,
                UpdatedAt = @UpdatedAt,
                UpdatedBy = @UpdatedBy
            WHERE ResourceId = @ResourceId AND IsDeleted = 0;";
        using var conn = _resolver.CreateConnection(clientId);
        return await conn.ExecuteAsync(sql, new { IsActive = isActive, UpdatedAt = updatedAt, UpdatedBy = updatedBy, ResourceId = resourceId });
    }

    public async Task<int> SetActiveByNameVersionAsync(Guid clientId, string name, int version, bool isActive, Guid updatedBy, DateTime updatedAt)
    {
        var resourceId = await FindResourceIdByNameVersionAsync(clientId, name, version);
        if (resourceId is null || resourceId == Guid.Empty)
            return 0;
        return await SetActiveByIdAsync(clientId, resourceId.Value, isActive, updatedBy, updatedAt);
    }
}