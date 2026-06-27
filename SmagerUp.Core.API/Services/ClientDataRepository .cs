using Dapper;
using SmagerUp.Core.API.DTOs;
using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;
using SmagerUp.Core.API.Models;
namespace SmagerUp.Core.API.Data.Client;

public class ClientDataRepository : IClientDataRepository {
    private readonly IClientDbResolver _clientDb;
    private readonly ClientActionsRepository _actions;

    public ClientDataRepository(IClientDbResolver clientDb, ClientActionsRepository sqlCommands) {
        _clientDb = clientDb;
        _actions = sqlCommands;
    }

    private async Task<object> GetDataResultAsync( GridReader multi){
        var datasets = new List<object>();

        while (!multi.IsConsumed)
        {
            datasets.Add((await multi.ReadAsync()).ToList());
        }

        object result = new { };

        if (datasets.Count > 1)
        {
            var last = (IEnumerable<object>) datasets[^1];
            result = last.FirstOrDefault() ?? new { };
            datasets.RemoveAt( datasets.Count - 1);
        }

        return new
        {
            isSuccess = true,
            result,
            datasets
        };
    }

    private async Task<object> GetActionResultAsync( GridReader multi) {
        object result = new { };

        if (!multi.IsConsumed)
        {
            var rows =  (await multi.ReadAsync())
                .ToList();

            result = rows.FirstOrDefault()
                ?? new { };
        }

        return new
        {
            isSuccess = true,
            result
        };
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

    private CommandType GetDbCommandType(string commandType) {
        return commandType switch {
            "P" => CommandType.StoredProcedure,
            "T" => CommandType.Text,
            _ => throw new Exception(
                $"Unsupported CommandType '{commandType}'.")
        };
    }

    private DynamicParameters BuildParameters(Guid userId,DataRequest request) {
        var p = new DynamicParameters();

        if (request.Parameters != null) {
            foreach (var item in request.Parameters) {
                p.Add(item.Key,item.Value);
            }
        }

        if (userId != Guid.Empty) {
            p.Add("UserId",userId);
        }

        if (request.Rows?.Any() == true) {
            var dt =ToDataTable(request.Rows);

            p.Add("tt",dt.AsTableValuedParameter());
        }

        return p;
    }

    private DataTable ToDataTable(List<Dictionary<string, object?>> rows) {
        var dt = new DataTable();

        if (!rows.Any()) return dt;

        foreach (var column in rows[0].Keys) {
            dt.Columns.Add(column);
        }

        foreach (var row in rows) {
            var dr = dt.NewRow();

            foreach (var item in row) {
                dr[item.Key] =
                    item.Value ?? DBNull.Value;
            }

            dt.Rows.Add(dr);
        }

        return dt;
    }

    private async Task<object> ExecuteGraphQlAsync(ActionInfo action,Dictionary<string, object>? parameters = null) {
        var config =JsonSerializer.Deserialize<Models.GraphQL.GraphQlConfig>(action.CommandText);

        if (config == null)
            throw new Exception("Invalid GraphQL configuration.");

        using var client = new HttpClient();

        if (config.Headers != null) {
            foreach (var h in config.Headers) {
                client.DefaultRequestHeaders.Add(h.Key,h.Value);
            }
        }

        var body = new {
            query = config.Query,
            variables = parameters ?? new()
        };

        var response =await client.PostAsJsonAsync(config.Endpoint,body);

        response.EnsureSuccessStatusCode();

        var json =await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("errors",out var errors)) {
            throw new Exception(errors.ToString());
        }

        if (doc.RootElement.TryGetProperty("data",out var data)) {
            return JsonSerializer.Deserialize<object>(data.GetRawText())!;
        }

        return new { };
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

