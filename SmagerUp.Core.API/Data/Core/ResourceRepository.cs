using Dapper;
using SmagerUp.Core.API.Models.Core;
using System.Data;

namespace SmagerUp.Core.API.Data.Core
{
    public class ResourceRepository
    {
        private readonly CoreDapperContext _db;
        public ResourceRepository(CoreDapperContext db) => _db = db;

        public async Task<IEnumerable<Resource>> GetResourcesByComponentAsync(Guid componentId)
        {
            const string sql = @"
                SELECT r.ResourceId, r.ResourceName, r.ResourceType, r.ResourceBody, r.Version, r.IsDeleted, r.CreatedAt
                FROM Resources r
                INNER JOIN ComponentResources cr ON r.ResourceId = cr.ResourceId
                WHERE cr.ComponentId = @componentId AND r.IsDeleted = 0 AND cr.IsActive = 1
                ORDER BY r.ResourceType;";

            using var conn = _db.CreateConnection();
            var result = await conn.QueryAsync<Resource>(sql, new { componentId });
            return result;
        }
    }
}