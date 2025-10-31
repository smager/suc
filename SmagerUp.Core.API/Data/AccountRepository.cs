using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data;
public class AccountRepository
{
    private readonly DapperContext _ctx;
    public AccountRepository(DapperContext ctx) => _ctx = ctx;

    public async Task<Account?> ValidateAsync(Guid accountId, string key)
    {
        var sql = "SELECT * FROM Accounts WHERE AccountId=@Id AND ApiKey=@Key";
        using var con = _ctx.CreateConnection();
        return await con.QueryFirstOrDefaultAsync<Account>(sql, new { Id = accountId, Key = key });
    }
}
