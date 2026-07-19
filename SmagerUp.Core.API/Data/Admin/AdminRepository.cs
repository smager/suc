using Dapper;
using SmagerUp.Core.API.Models;
using System.Data;

namespace SmagerUp.Core.API.Data.Admin;

public class AdminRepository
{
    private readonly AdminDbContext _ctx;
    public AdminRepository(AdminDbContext ctx) => _ctx = ctx;

    public async Task<Models.Client.App> GetAppByApiKeyAsync(string ApiKey )
    {
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Models.Client.App>("apps_sel", new { ApiKey }, commandType:CommandType.StoredProcedure);
    }

    public async Task<User?> GetUser(string UserName)
    {
        using var con = _ctx.CreateConnection();
        return await con.QuerySingleOrDefaultAsync<User>("users_sel", new { UserName }, commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetUserById(Guid UserId)
    {
        using var con = _ctx.CreateConnection();
        return await con.QuerySingleOrDefaultAsync<User>("users_sel", new { UserId }, commandType: CommandType.StoredProcedure);
    }
}
