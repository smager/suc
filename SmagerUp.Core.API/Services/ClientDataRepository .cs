using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SmagerUp.Core.API.DTOs;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Client;

public class ClientDataRepository :BaseDataRepository, IClientDataRepository {
    private readonly IClientDbResolver _clientDb;

    public ClientDataRepository(IClientDbResolver clientDb) {
        _clientDb = clientDb;
    }

    public async Task<object> ExecuteAsync(string ApiKey,Guid userId,DataRequest request){
        try {
            this.connection = _clientDb.CreateConnection(ApiKey);
            this.ApiKey= ApiKey;

            var action =   await GetByCodeAsync(request.ActionCode);
            return  await RunActionAsync(userId,request,action);
        }
        catch (Exception ex)
        {
            return new
            {
                ok = false,
                errMsg =  ex.Message
            };
        }

    }

 
   
}

