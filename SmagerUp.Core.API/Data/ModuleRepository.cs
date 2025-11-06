using Dapper;
using SmagerUp.Core.API.Models;
using System.Data;

namespace SmagerUp.Core.API.Data
{
    public class ModuleRepository
    {
        private readonly DapperContext _db;
        public ModuleRepository(DapperContext db) => _db = db;

        public async Task<Module?> GetModuleAsync(string name, string version)
        {
            const string sql = @"
                SELECT ModuleId, Name, Version, LicenseTypeId, Price, ContentGroupId, IsActive, CreatedAt
                FROM Modules
                WHERE Name = @name AND Version = @version AND IsActive = 1 AND IsDeleted=0;";

            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Module>(sql, new { name, version });
        }

        public async Task<IEnumerable<Module>> GetModulesForAccountAsync(Guid? accountId)
        {
            const string sql = @"
        SELECT m.ModuleId, m.Name, m.Version, m.LicenseTypeId, 
               m.Price, m.ContentGroupId, m.IsActive, m.CreatedAt
        FROM Modules m
        INNER JOIN AccountModules am ON am.ModuleId = m.ModuleId
        WHERE am.AccountId = @accountId AND am.IsActive = 1 AND m.IsDeleted=0;";

            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Module>(sql, new { accountId });
        }

    }
}
