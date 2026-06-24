using Dapper;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.DTOs;
using System.Data;
using System.Text.Json;

namespace SmagerUp.Core.API.Data.Client;

public class CoreDataRepository : ICoreDataRepository
{
    private readonly CoreDbContext _ctx;
    private readonly CoreSqlCommandsRepository _sqlCommands;

    public CoreDataRepository(CoreDbContext ctx,CoreSqlCommandsRepository sqlCommands)
    {
        _ctx = ctx;
        _sqlCommands = sqlCommands;
    }

    public async Task<object> GetDataAsync(Guid? userId,DataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SqlCode))
            {
                return new
                {
                    isSuccess = false,
                    errMsg = "SqlCode is required."
                };
            }

            var sqlCmd = await _sqlCommands.GetByCodeAsync(request.SqlCode);

            if (sqlCmd == null)
            {
                return new
                {
                    isSuccess = false,
                    errMsg = $"SqlCode '{request.SqlCode}' not found."
                };
            }

            using var conn = _ctx.CreateConnection();

            var p =
                BuildParameters(
                    userId,
                    request);

            using var multi =
                await conn.QueryMultipleAsync(
                    sqlCmd.SqlCmdText,
                    p,
                    commandType:
                        sqlCmd.IsProcedure == "Y"
                            ? CommandType.StoredProcedure
                            : CommandType.Text);

            var datasets = new List<object>();

            while (!multi.IsConsumed)
            {
                datasets.Add(
                    (await multi.ReadAsync()).ToList());
            }

            object result = new { };

            if (datasets.Count > 1)
            {
                var lastDataset =
                    (IEnumerable<object>)datasets[^1];

                result =
                    lastDataset.FirstOrDefault()
                    ?? new { };

                datasets.RemoveAt(
                    datasets.Count - 1);
            }

            return new
            {
                isSuccess = true,
                result,
                datasets
            };
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

    public async Task<object> ExecuteCmdAsync( Guid? userId,DataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SqlCode))
            {
                return new
                {
                    isSuccess = false,
                    errMsg = "SqlCode is required."
                };
            }

            var sqlCmd =
                await _sqlCommands.GetByCodeAsync(request.SqlCode);

            if (sqlCmd == null)
            {
                return new
                {
                    isSuccess = false,
                    errMsg = $"SqlCode '{request.SqlCode}' not found."
                };
            }

            using var conn = _ctx.CreateConnection();

            var p =BuildParameters(userId,request);

            using var multi =
                await conn.QueryMultipleAsync(
                    sqlCmd.SqlCmdText,
                    p,
                    commandType:
                        sqlCmd.IsProcedure == "Y"
                            ? CommandType.StoredProcedure
                            : CommandType.Text);

            object result = new { };

            if (!multi.IsConsumed)
            {
                var rows =
                    (await multi.ReadAsync())
                    .ToList();

                result =
                    rows.FirstOrDefault()
                    ?? new { };
            }

            return new
            {
                isSuccess = true,
                result
            };
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

    private DynamicParameters BuildParameters(  Guid? userId,DataRequest request)
    {
        var p = new DynamicParameters();


        if (request.Parameters != null)
        {
            foreach (var item in request.Parameters)
            {

                object value = item.Value;

                if (value is JsonElement je)
                {
                    value = ConvertJsonElement(je);
                }

                p.Add(
                    item.Key,
                    value);
            }
        }

        if (userId != Guid.Empty)
        {
            p.Add(
                "UserId",
                userId);
        }

        if (request.Rows?.Any() == true)
        {
            var dt =
                ToDataTable(
                    request.Rows);

            p.Add(
                "tt",
                dt.AsTableValuedParameter());
        }

        return p;
    }

    private object? ConvertJsonElement(
    JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                return element.GetString();

            case JsonValueKind.Number:

                if (element.TryGetInt32(out int i))
                    return i;

                if (element.TryGetInt64(out long l))
                    return l;

                if (element.TryGetDecimal(out decimal d))
                    return d;

                return element.ToString();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            case JsonValueKind.Null:
                return null;

            default:
                return element.ToString();
        }
    }

    private DataTable ToDataTable( List<Dictionary<string, object?>> rows)
    {
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
}