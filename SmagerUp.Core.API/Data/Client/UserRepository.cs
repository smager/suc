using Dapper;

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
        var sql = "SELECT u.*,r.RoleId,r.RoleName FROM dbo.su_Users u INNER JOIN dbo.su_Roles r on u.RoleId=r.RoleId WHERE u.UserName=@UserName";
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>(sql, new { UserName});
    }


    public async Task<Models.Client.User?> GetUserByIdAsync(Guid ClientId, Guid UserId)
    {
        var sql = "SELECT * FROM dbo.su_Users WHERE UserId=@UserId";
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>(sql, new { UserId});
    }

}