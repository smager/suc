using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmagerUp.Core.API.Data.Client;
using SmagerUp.Core.API.DTOs;
using System.Data;

namespace SmagerUp.Core.API.Controllers;

[ApiController]
[Route("api/client/data")]
[Authorize]
public class DataController : SucController
{
    private readonly IClientDbResolver _clientDb;
    private readonly SqlCommandsRepository _sqlCommands;

    public DataController(IClientDbResolver clientDb,SqlCommandsRepository sqlCommands)
    {
        _clientDb = clientDb;
        _sqlCommands = sqlCommands;
    }

    [HttpPost("getdata")]
    public async Task<IActionResult> GetData(
       [FromBody] DataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SqlCode))
                return Ok(new
                {
                    isSuccess = false,
                    errMsg = "SqlCode is required."
                });

            var sqlCmd = await _sqlCommands.GetByCodeAsync(
                ClientId,
                request.SqlCode);

            if (sqlCmd == null)
                return Ok(new
                {
                    isSuccess = false,
                    errMsg = $"SqlCode '{request.SqlCode}' not found."
                });

            using var conn =
                _clientDb.CreateConnection(ClientId);

            var p = BuildParameters(request);

            using var multi = await conn.QueryMultipleAsync(sqlCmd.SqlCmdText,p,commandType:sqlCmd.IsProcedure == "Y" ? CommandType.StoredProcedure: CommandType.Text);

            var datasets = new List<object>();

            while ( ! multi.IsConsumed)
            {
                datasets.Add(
                    (await multi.ReadAsync()).ToList()
                );
            }

            object result = new { };

            if (datasets.Count > 1)
            {
                var lastDataset = datasets[^1] as IEnumerable<dynamic>;
                result =  lastDataset?.FirstOrDefault() ?? new { };
                datasets.RemoveAt(datasets.Count - 1);
            }

            return Ok(new
            {
                isSuccess = true,
                result,
                datasets
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                isSuccess = false,
                errMsg = ex.Message
            });
        }
    }

    [HttpPost("executecmd")]
    public async Task<IActionResult> ExecuteCmd([FromBody] DataRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.SqlCode))
                return Ok(new
                {
                    isSuccess = false,
                    errMsg = "SqlCode is required."
                });

            var sqlCmd = await _sqlCommands.GetByCodeAsync(ClientId, request.SqlCode);

            if (sqlCmd == null)
                return Ok(new
                {
                    isSuccess = false,
                    errMsg = $"SqlCode '{request.SqlCode}' not found."
                });

            using var conn =
                _clientDb.CreateConnection(ClientId);

            var p = BuildParameters(request);
 

            var result= await conn.QueryFirstOrDefaultAsync(
                     sqlCmd.SqlCmdText
                    ,p
                    ,commandType:
                        sqlCmd.IsProcedure == "Y"
                        ? CommandType.StoredProcedure
                        : CommandType.Text);

            return Ok(new
            {
                isSuccess = true,
                result
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                isSuccess = false,
                errMsg = ex.Message
            });
        }
    }

    private DynamicParameters BuildParameters(
        DataRequest request)
    {
        var p = new DynamicParameters();

        if (request.Parameters != null)
        {
            foreach (var item in request.Parameters)
            {
                p.Add(item.Key, item.Value);
            }
        }

        if (UserId != Guid.Empty)
        {
            p.Add("UserId", UserId);
        }

        if (request.Rows?.Any() == true)
        {
            var dt = ToDataTable(request.Rows);

            p.Add("tt",dt.AsTableValuedParameter());
        }


        return p;
    }

    private DataTable ToDataTable(
    List<Dictionary<string, object>> rows)
    {
        var dt = new DataTable();

        if (!rows.Any())
            return dt;

        foreach (var col in rows[0].Keys)
        {
            dt.Columns.Add(col);
        }

        foreach (var row in rows)
        {
            var dr = dt.NewRow();

            foreach (var col in row.Keys)
            {
                dr[col] = row[col] ?? DBNull.Value;
            }

            dt.Rows.Add(dr);
        }

        return dt;
    }
}