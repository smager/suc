 
using Dapper;
using System.Data;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data.Client;
public class ClientRepository
{
    private readonly IClientDbResolver _resolver;

    public ClientRepository(IClientDbResolver resolver) => _resolver = resolver;

     public async Task<User?> GetUserByUserNameAsync(string ApiKey, string UserName)
    {
        using var con = _resolver.CreateConnection(ApiKey);
        return await con.QueryFirstOrDefaultAsync<User>("su_Users_sel", new { UserName }, commandType: CommandType.StoredProcedure);

    }


    public async Task<User?> GetUserByIdAsync(string ApiKey, Guid UserId)
    {
        using var con = _resolver.CreateConnection(ApiKey);
        return await con.QueryFirstOrDefaultAsync<User>("su_Users_sel", new { UserId}, commandType: CommandType.StoredProcedure);
    }
      
}


 