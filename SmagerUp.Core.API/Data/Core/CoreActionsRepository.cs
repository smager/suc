using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data.Core;

public class CoreActionsRepository
{
    private readonly CoreDbContext _ctx;
    public CoreActionsRepository(CoreDbContext ctx) => _ctx = ctx;

    public async Task<ActionInfo?> GetByCodeAsync(string ActionCode)
    {
        using var con = _ctx.CreateConnection();

        return await con.QueryFirstOrDefaultAsync<ActionInfo>("Actions_sel", new { ActionCode },commandType:System.Data.CommandType.StoredProcedure);
    }


}

