using Dapper;
using System.Data;

namespace SmagerUp.Core.API.Data.Core;

public class CoreRepository
{
    private readonly CoreDbContext _ctx;
    public CoreRepository(CoreDbContext ctx) => _ctx = ctx;

    public async Task<Models.Core.Client?> GetClientByIdAsync(Guid ClientId)
    {
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Models.Core.Client>("Clients_sel", new { ClientId }, commandType:CommandType.StoredProcedure);
    }
}
