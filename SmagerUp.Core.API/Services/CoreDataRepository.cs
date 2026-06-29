using Dapper;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs;
using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data.Client;

public class CoreDataRepository : BaseDataRepository, ICoreDataRepository
{
    private readonly CoreDbContext _ctx;
    private readonly CoreActionsRepository _actions;

    public CoreDataRepository(CoreDbContext ctx,CoreActionsRepository sqlCommands)
    {
        _ctx = ctx;
        _actions = sqlCommands;
    }


    private async Task<Models.ActionInfo> GetActionAsync(string? actionCode) {
        if (string.IsNullOrWhiteSpace(actionCode))
            throw new Exception("ActionCode is required.");

        var action = await _actions.GetByCodeAsync(actionCode);

        if (action == null)
            throw new Exception($"ActionCode '{actionCode}' not found.");

        return action;
    }
    private async Task<T> ExecuteRequestAsync<T>( Guid userId,Models.ActionInfo action,DataRequest request, Func<GridReader, Task<T>> handler) {
        using var conn = _ctx.CreateConnection();
        var p = BuildParameters(userId,request);
        using var multi = await conn.QueryMultipleAsync(action.CommandText,p,commandType:GetDbCommandType(action.CommandType));
        return await handler(multi);
    }
    public async Task<object> ExecuteAsync(Guid userId,DataRequest request){
        var action =   await GetActionAsync(request.ActionCode);

        return action.ActionType switch{
            "Q" => await RunQueryAsync(userId,request),

            "C" => await RunCommandAsync(userId,request),

            _ => throw new Exception(
                    $"Unsupported ActionType '{action.ActionType}'.")
        };
    }
    public async Task<object> RunQueryAsync( Guid userId, DataRequest request) {
    try
    {
        var action =await GetActionAsync(request.ActionCode);

        switch (action.CommandType)
        {
            case "P":
            case "T": return await ExecuteRequestAsync( userId, action, request, GetDataResultAsync);
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
    public async Task<object> RunCommandAsync(Guid userId,DataRequest request) {
        try
        {
            var action = await GetActionAsync( request.ActionCode);

            switch (action.CommandType)
            {
                case "P":
                case "T":   return await ExecuteRequestAsync(userId,action,request, GetActionResultAsync);

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