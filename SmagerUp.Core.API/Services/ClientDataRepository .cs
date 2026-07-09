using Dapper;
using SmagerUp.Core.API.DTOs;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Client;

public class ClientDataRepository :BaseDataRepository, IClientDataRepository {
    private readonly IClientDbResolver _clientDb;

    public ClientDataRepository(IClientDbResolver clientDb) {
        _clientDb = clientDb;
    }

    public async Task<object> ExecuteAsync(Guid clientId,Guid userId,DataRequest request){
        try {
             this.connection = _clientDb.CreateConnection(clientId);
            this.clientId = clientId;

            var action =   await this.GetByCodeAsync(request.ActionCode);
            return action.ActionType.ToUpper() switch{
                "Q" => await RunQueryAsync(userId,request,action),

                "C" => await RunCommandAsync(userId,request, action),

                _ => throw new Exception(
                        $"Unsupported ActionType '{action.ActionType}'.")
            }; 
        }
        catch (Exception ex)
        {
            return new
            {
                isSuccess = false,
                errMsg =  ex.Message
            };
        }

    }

 
   
}

