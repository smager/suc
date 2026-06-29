using Dapper;
using SmagerUp.Core.API.DTOs;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Client;

public class ClientDataRepository :BaseDataRepository, IClientDataRepository {
    private readonly IClientDbResolver _clientDb;
    private readonly ClientActionsRepository _actions;

    public ClientDataRepository(IClientDbResolver clientDb, ClientActionsRepository sqlCommands) {
        _clientDb = clientDb;
        _actions = sqlCommands;
    }

    private async Task<Models.ActionInfo> GetActionAsync(Guid clientId, string? actionCode) {
        if (string.IsNullOrWhiteSpace(actionCode))
            throw new Exception("ActionCode is required.");

        var action = await _actions.GetByCodeAsync(clientId,actionCode);

        if (action == null)
            throw new Exception($"ActionCode '{actionCode}' not found.");

        return action;
    }

    private async Task<T> ExecuteRequestAsync<T>(Guid clientId, Guid userId,Models.ActionInfo action,DataRequest request, Func<GridReader, Task<T>> handler) {
        using var conn = _clientDb.CreateConnection(clientId);
        var p = BuildParameters(userId,request);
        using var multi = await conn.QueryMultipleAsync(action.CommandText,p,commandType:GetDbCommandType(action.CommandType));
        return await handler(multi);
    }

    public async Task<object> ExecuteAsync(Guid clientId,Guid userId,DataRequest request){
        var action =   await GetActionAsync(clientId,request.ActionCode);

        return action.ActionType switch{
            "Q" => await RunQueryAsync(
                        clientId,
                        userId,
                        request),

            "C" => await RunCommandAsync(
                        clientId,
                        userId,
                        request),

            _ => throw new Exception(
                    $"Unsupported ActionType '{action.ActionType}'.")
        };
    }

    public async Task<object> RunQueryAsync(Guid clientId, Guid userId, DataRequest request) {
    try
    {
        var action =await GetActionAsync(clientId,request.ActionCode);

        switch (action.CommandType)
        {
            case "P":
            case "T": return await ExecuteRequestAsync( clientId, userId, action, request, GetDataResultAsync);
            case "G":  return new {
                        isSuccess = true,
                        result = new { },
                        datasets = new[] {
                            await ExecuteGraphQlAsync(action,request.Parameters)
                        }
                    };

            default:
                throw new Exception(
                    $"Unsupported CommandType '{action.CommandType}'.");
        }
    }
    catch (Exception ex)
    {
        return new
        {
            isSuccess = false,
            errMsg = ex.Message
        };
    }
}

    public async Task<object> RunCommandAsync(Guid clientId,Guid userId,DataRequest request) {
    try
    {
        var action =
            await GetActionAsync(  clientId, request.ActionCode);

        switch (action.CommandType)
        {
            case "P":
            case "T":   return await ExecuteRequestAsync(clientId,userId,action,request, GetActionResultAsync);

            case "G":   return new {
                            isSuccess   = true,  
                            result      = await ExecuteGraphQlAsync( action,  request.Parameters)
                        };

            default:    throw new Exception($"Unsupported CommandType '{action.CommandType}'.");
        }
    }
    catch (Exception ex) {
        return new {
            isSuccess = false,
            errMsg = ex.Message
        };
    }
}
   
}

