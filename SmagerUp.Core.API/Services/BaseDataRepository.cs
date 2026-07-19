using Dapper;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Models;
using System.Data;
using System.Data.Common;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data;

public abstract class BaseDataRepository {
    protected IDbConnection connection;
    protected string ApiKey;

    public async Task<ActionInfo?> GetByCodeAsync(string? ActionCode)
    {
        var sp = "dbo.su_actions_sel";
        if (ApiKey == string.Empty) sp = "dbo.actions_sel";

        return await this.connection.QueryFirstOrDefaultAsync<ActionInfo>(sp, new { ActionCode }, commandType:CommandType.StoredProcedure);
    }
    protected async Task<object> ExecuteRequestAsync(Guid userId,ActionInfo action,DataRequest request)
    {
        var p = BuildParameters(userId, request);

        using var reader =   (DbDataReader)await connection.ExecuteReaderAsync(action.CommandText,p,commandType: GetDbCommandType(action.CommandType));

        return await BuildResponseAsync(reader, action);
    }
    protected async Task<ActionInfo> GetActionAsync( string? actionCode) {
        if (string.IsNullOrWhiteSpace(actionCode))
            throw new Exception("ActionCode is required.");

        var action = await GetByCodeAsync(actionCode);

        if (action == null) throw new Exception($"ActionCode '{actionCode}' not found.");

        return action;
    }
    protected async Task<object> RunActionAsync(Guid userId,DataRequest request, ActionInfo action) {
        try
        {
            switch (action.CommandType)
            {
                case "P":
                case "T":   return await ExecuteRequestAsync(userId,action,request);

                case "G":   return new {
                                ok   = true,  
                                result  = await ExecuteGraphQlAsync( action,  request.Parameters)
                            };
                default:    throw new Exception($"Unsupported CommandType '{action.CommandType}'.");
            }
        }
        catch (Exception ex) {
            return new {
                ok = false,
                 errMsg =  SanitizeSqlError(ex.Message)
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
                object value = item.Value;

                if (value is JsonElement json)
                {
                    value = json.ValueKind switch
                    {
                        JsonValueKind.String => json.GetString(),
                        JsonValueKind.Number =>
                            json.TryGetInt64(out var l)
                                ? l
                                : json.GetDecimal(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => DBNull.Value,
                        JsonValueKind.Undefined => DBNull.Value,
                        JsonValueKind.Array => json.ToString(),
                        JsonValueKind.Object => json.ToString(),
                        _ => json.ToString()
                    };
                }

                p.Add(item.Key, value);
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
    protected async Task<object> BuildResponseAsync(DbDataReader reader, ActionInfo action){
        switch (action.ResponseFormat?.ToUpper())
        {
            case "R":
                return await GetRowsAsync(reader);

            case "C":
                return await GetColumnsAsync(reader);

            case "O":
                return await GetObjectAsync(reader);

            case "S":
                return await GetScalarAsync(reader);

            default:
                throw new Exception(
                    $"Unsupported ResponseFormat '{action.ResponseFormat}'.");
        }
    }
    protected async Task<object> GetRowsAsync(DbDataReader reader)
    {
        var result = new List<List<Dictionary<string, object?>>>();
        do
        {
            var rows = new List<Dictionary<string, object?>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(
                        reader.GetName(i),
                        await reader.IsDBNullAsync(i)
                            ? null
                            : reader.GetValue(i));
                }

                rows.Add(row);
            }

            result.Add(rows);

        } while (await reader.NextResultAsync());

        return new
        {
            ok = true,
            result
        };
    }
    protected async Task<object> GetObjectAsync(DbDataReader reader)
    {
        Dictionary<string, object?>? row = null;

        if (await reader.ReadAsync())
        {
            row = new();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row.Add(
                    reader.GetName(i),
                    await reader.IsDBNullAsync(i)
                        ? null
                        : reader.GetValue(i));
            }
        }

        return new
        {
            ok = true,
            result = row
        };
    }
    protected async Task<object> GetScalarAsync(DbDataReader reader)
    {
        object? value = null;

        if (await reader.ReadAsync())
        {
            value = await reader.IsDBNullAsync(0)
                ? null
                : reader.GetValue(0);
        }

        return new
        {
            ok = true,
            result = value
        };
    }
    protected async Task<object> GetColumnsAsync(DbDataReader reader)
    {
        var result = new List<object>();

        do
        {
            var columns = Enumerable
                .Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .ToArray();

            var rows = new List<object[]>();

            while (await reader.ReadAsync())
            {
                var values = new object[reader.FieldCount];

                reader.GetValues(values);

                rows.Add(values);
            }

            result.Add(new
            {
                columns,
                rows
            });

        } while (await reader.NextResultAsync());

        return new
        {
            ok = true,
            result
        };
    }

}
 
