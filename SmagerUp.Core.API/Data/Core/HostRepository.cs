using Dapper;
using SmagerUp.Core.API.Models.Client;
using System.Data;
 

namespace SmagerUp.Core.API.Data.Core;
public class HostRepository
{
    private readonly CoreDapperContext _ctx;
    public HostRepository(CoreDapperContext ctx) => _ctx = ctx;

    public async Task<Models.Core.Host?> GetHostInfo()
    {
        using var con = _ctx.CreateConnection();
        return  await con.QuerySingleOrDefaultAsync<Models.Core.Host>("host_sel",null, commandType: CommandType.StoredProcedure);
    }


    public async Task<User?> GetHostUser(string UserName)
    {
        using var con = _ctx.CreateConnection();
        return await con.QuerySingleOrDefaultAsync<User>("host_user_sel", new { UserName }, commandType: CommandType.StoredProcedure);
    }
}