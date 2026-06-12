using Dapper;
using SmagerUp.Core.API.Models.Core;
using System.Data;

namespace SmagerUp.Core.API.Data.Core
{
    public class ComponentRepository
    {
        private readonly CoreDapperContext _db;
        public ComponentRepository(CoreDapperContext db) => _db = db;

        // now accepts int version (DB column is int)
        public async Task<Component?> GetComponentAsync(string name, string version)
        {
            const string sql = @"
                SELECT ComponentId, Name, Version, ComponentTypeId, LicenseTypeId, Price, IsActive, IsDeleted, CreatedAt
                FROM Components
                WHERE Name = @name AND IsActive = 1 AND IsDeleted = 0;";

            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Component>(sql, new { name, version });
        }

        public async Task<IEnumerable<Component>> GetClientComponentsAsync(Guid? ClientId)
        {
            const string sql = @"
                SELECT c.ComponentId, c.Name, c.Version, c.ComponentTypeId, c.LicenseTypeId, 
                       c.Price, c.IsActive, c.IsDeleted, c.CreatedAt
                FROM Components c
                INNER JOIN ClientComponents ac ON ac.ComponentId = c.ComponentId
                WHERE ac.ClientId = @ClientId AND ac.IsActive = 1 AND c.IsDeleted = 0;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Component>(sql, new { ClientId });
        }

        public async Task<IEnumerable<Component>> GetAllAsync()
        {
            const string sql = @"
                SELECT ComponentId, Name, Version, ComponentTypeId, LicenseTypeId, Price, IsActive, IsDeleted, CreatedAt
                FROM Components
                WHERE IsActive = 1 AND IsDeleted = 0
                ORDER BY Name, Version;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Component>(sql);
        }
    }
}