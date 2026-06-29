using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data.Client;

public class ClientActionsRepository
{
    private readonly IClientDbResolver _db;

    public ClientActionsRepository(   IClientDbResolver db)
    {
        _db = db;
    }

    public async Task<ActionInfo?> GetByCodeAsync(Guid clientId, string ActionCode)
    {

        using var conn =_db.CreateConnection(clientId);

        return await conn.QueryFirstOrDefaultAsync<ActionInfo>("dbo.su_actions_sel", new { ActionCode = ActionCode },commandType:System.Data.CommandType.StoredProcedure);
    }
}
