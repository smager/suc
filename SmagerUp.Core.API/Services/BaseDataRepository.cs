
using Dapper;
using SmagerUp.Core.API.DTOs;
using SmagerUp.Core.API.Models;
using System.Data;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace SmagerUp.Core.API.Data;

public abstract class BaseDataRepository
{
    protected async Task<object> GetDataResultAsync(GridReader multi)
    {
        var datasets = new List<object>();

        while (!multi.IsConsumed)
        {
            datasets.Add((await multi.ReadAsync()).ToList());
        }

        object result = new { };

        if (datasets.Count > 1)
        {
            var last = (IEnumerable<object>)datasets[^1];
            result = last.FirstOrDefault() ?? new { };
            datasets.RemoveAt(datasets.Count - 1);
        }

        return new
        {
            isSuccess = true,
            result,
            datasets
        };
    }

    protected async Task<object> GetActionResultAsync(GridReader multi)
    {
        object result = new { };

        if (!multi.IsConsumed)
        {
            var rows = (await multi.ReadAsync()).ToList();
            result = rows.FirstOrDefault() ?? new { };
        }

        return new
        {
            isSuccess = true,
            result
        };
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
}
 
