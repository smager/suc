using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data;
public class ClientRepository
{
    private readonly CoreDapperContext _ctx;
    public ClientRepository(CoreDapperContext ctx) => _ctx = ctx;

    public async Task<Client?> ValidateAsync(Guid ClientId, string key)
    {
        var sql = "SELECT * FROM Clients WHERE ClientId=@Id AND ApiKey=@Key";
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Client>(sql, new { Id = ClientId, Key = key });
    }
}
