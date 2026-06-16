using Dapper;

namespace SmagerUp.Core.API.Data.Core;
public class ClientRepository
{
    private readonly CoreDapperContext _ctx;
    public ClientRepository(CoreDapperContext ctx) => _ctx = ctx;

    public async Task<Models.Core.Client?> GetClientByIdAsync(Guid ClientId)
    {
        var sql = "SELECT * FROM Clients WHERE ClientId=@Id";
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Models.Core.Client>(sql, new { Id = ClientId});
    }
}
