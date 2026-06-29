using Dapper;
using System.Data;

namespace SmagerUp.Core.API.Data.Client;

public class UserRepository
{
    private readonly IClientDbResolver _resolver;

    public UserRepository(IClientDbResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task<Models.Client.User?> GetClientByUserNameAsync(Guid ClientId, string UserName)
    {
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>("su_Users_sel", new { UserName }, commandType: CommandType.StoredProcedure);

    }


    public async Task<Models.Client.User?> GetUserByIdAsync(Guid ClientId, Guid UserId)
    {
        using var con = _resolver.CreateConnection(ClientId);
        return await con.QueryFirstOrDefaultAsync<Models.Client.User>("su_Users_sel", new { UserId}, commandType: CommandType.StoredProcedure);
    }

}