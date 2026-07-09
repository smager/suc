using Dapper;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Models;
using System.Data;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data;

 

public abstract class BaseDataRepository {
    protected IDbConnection connection;
    protected Guid clientId;

     
    /// <summary>Query multiple datasets from the database and return them as a list of objects.</summary>
    protected async Task<object> GetDataResultAsync(GridReader multi, string ActionType)
    { 
        try{
            var dataSets = new List<List<object>>();
            while (!multi.IsConsumed)
            {
                dataSets.Add((await multi.ReadAsync()).ToList());
            }

            return new {
                isSuccess = true,
                ActionType,
                dataSets
            }; 

        }
        catch (Exception ex)
      {
        return new {
            isSuccess = false,
            errorMessage = ex.Message
        };
        } 
    }

    public async Task<ActionInfo?> GetByCodeAsync(string? ActionCode)
    {
        var sp = "dbo.su_actions_sel";
        if (clientId == Guid.Empty) sp = "dbo.actions_sel";

        return await this.connection.QueryFirstOrDefaultAsync<ActionInfo>(sp, new { ActionCode = ActionCode }, commandType: System.Data.CommandType.StoredProcedure);
    }

    protected async Task<T> ExecuteRequestAsync<T>(Guid userId,Models.ActionInfo action,DataRequest request, Func<GridReader,string, Task<T>> handler) {
       // using var conn = _clientDb.CreateConnection(clientId);
        var p = BuildParameters(userId,request);
        using var multi = await connection.QueryMultipleAsync(action.CommandText,p,commandType:GetDbCommandType(action.CommandType));
        return await handler(multi, action.ActionType.ToUpper());
    }

    protected async Task<Models.ActionInfo> GetActionAsync( string? actionCode) {
    if (string.IsNullOrWhiteSpace(actionCode))
        throw new Exception("ActionCode is required.");

    var action = await GetByCodeAsync(actionCode);

    if (action == null)
        throw new Exception($"ActionCode '{actionCode}' not found.");

        return action;
    }

    protected async Task<object> RunQueryAsync(Guid userId, DataRequest request, ActionInfo action) {
    try
    {
       // var action =await GetActionAsync(request.ActionCode);

        switch (action.CommandType)
        {
            case "P":
            case "T": return await ExecuteRequestAsync(userId, action, request, GetDataResultAsync);
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
             errMsg =  SanitizeSqlError(ex.Message)
        };
    }
}

    protected async Task<object> RunCommandAsync(Guid userId,DataRequest request, ActionInfo action) {
    try
    {
        //var action =await GetActionAsync( request.ActionCode);

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
             errMsg =  SanitizeSqlError(ex.Message)
        };
    }
}
   

    /// <summary>Query a single dataset from the database and return it as an object.</summary>
    protected async Task<object> GetActionResultAsync(GridReader multi,string ActionType)
    {
        try{
            object data = new { };
            if (!multi.IsConsumed)
            {
                var rows = (await multi.ReadAsync()).ToList();
                data = rows.FirstOrDefault() ?? new { };
            }

            return new {
                isSuccess = true,
                ActionType,
                returnInfo = data
            }; 

        } catch (Exception ex) {
            return new {
                isSuccess = false,
                errorMessage = ex.Message
            };
        } 
    }

    protected DynamicParameters BuildParameters(Guid userId,DataRequest request)
    {
        var p = new DynamicParameters();

        if (request.Parameters != null)
        {
            foreach (var item in request.Parameters)
            {
                p.Add(item.Key, item.Value);
            }
        }

        if (userId != Guid.Empty)
        {
            p.Add("UserId", userId);
        }

        if (request.Rows?.Any() == true)
        {
            var dt = ToDataTable(request.Rows);
            p.Add("tt", dt.AsTableValuedParameter());
        }

        return p;
    }

    protected DataTable ToDataTable(List<Dictionary<string, object?>> rows){
        var dt = new DataTable();

        if (!rows.Any())
            return dt;

        foreach (var column in rows[0].Keys)
        {
            dt.Columns.Add(column);
        }

        foreach (var row in rows)
        {
            var dr = dt.NewRow();

            foreach (var item in row)
            {
                dr[item.Key] =
                    item.Value ?? DBNull.Value;
            }

            dt.Rows.Add(dr);
        }

        return dt;
    }

    protected CommandType GetDbCommandType(string commandType){
        return commandType switch
        {
            "P" => CommandType.StoredProcedure,
            "T" => CommandType.Text,
            _ => throw new Exception(
                $"Unsupported CommandType '{commandType}'.")
        };
    }

    protected async Task<object> ExecuteGraphQlAsync(ActionInfo action,Dictionary<string, object>? parameters = null)    {
        var config =JsonSerializer.Deserialize<SmagerUp.Core.API.Models.GraphQL.GraphQlConfig>(action.CommandText);

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

    
    protected string SanitizeSqlError(string message){
        if (string.IsNullOrWhiteSpace(message))
            return message;

            var match = Regex.Match(message,@"expects parameter '@?(\w+)'",RegexOptions.IgnoreCase);

        if (match.Success)
        {
            return $"Required field '{match.Groups[1].Value}' was not supplied.";
        }

        return message;
    }
}
 
