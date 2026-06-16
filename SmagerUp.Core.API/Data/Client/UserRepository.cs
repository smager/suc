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

    public async Task<Models.Client.User?> GetClientUserAsync(Guid ClientId, string UserName)
    {
        var sql = "SELECT u.*,r.RoleId,r.RoleName FROM Users u INNER JOIN Roles r on u.RoleId=r.RoleId WHERE u.UserName=@UserName";
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>(sql, new { UserName});
    }


    public async Task<Models.Client.User?> GetUserByIdAsync(Guid ClientId, Guid UserId)
    {
        var sql = "SELECT * FROM Users WHERE UserId=@UserId";
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>(sql, new { UserId});
    }

}