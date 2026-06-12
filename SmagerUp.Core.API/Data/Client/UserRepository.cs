using Dapper;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SmagerUp.Core.API.Data.Client;
public class UserRepository
{
    private readonly IClientDbResolver _resolver;

    public UserRepository(IClientDbResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<Models.Client.User?> ValidateClientUserAsync(Guid? ClientId, string UserName, string Password)
    {
        var sql = "SELECT * FROM Users WHERE UserName=@UserName and Password=@Password";
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>(sql, new { UserName, Password });
    }

   
}