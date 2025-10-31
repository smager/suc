using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data;
public class ModuleRepository
{
    private readonly DapperContext _ctx;
    public ModuleRepository(DapperContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Module>> GetModulesForAccount(Guid accountId)
    {
        var sql = @"SELECT m.* FROM UserModules um
                    JOIN Modules m ON um.ModuleId = m.ModuleId
                    WHERE um.AccountId=@AccountId AND um.IsActive=1 AND m.IsActive=1";
        using var con = _ctx.CreateConnection();
        return await con.QueryAsync<Module>(sql, new { AccountId = accountId });
    }

    public async Task<Module?> GetModuleAsync(string name, string version)
    {
        var sql = "SELECT * FROM Modules WHERE Name=@Name AND Version=@Version AND IsActive=1";
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Module>(sql, new { Name = name, Version = version });
    }
}
