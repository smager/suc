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
        //_clientDb.ClientId = ClientId;
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

            var sqlCmd = await _sqlCommands.GetByCodeAsync(ClientId,request.SqlCode);

            if (sqlCmd == null)
                return Ok(new
                {
                    isSuccess = false,
                    errMsg = $"SqlCode '{request.SqlCode}' not found."
                });

            using var conn =
                _clientDb.CreateConnection(ClientId);

            var p = BuildParameters(request);

            p.Add(
                "return_value",
                direction: ParameterDirection.ReturnValue);

            using var multi = await conn.QueryMultipleAsync(
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
                    (await multi.ReadAsync()).ToList()
                );
            }

            return Ok(new
            {
                isSuccess = true,
                returnValue = p.Get<object?>("return_value"),
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

            p.Add(
                "return_value",
                dbType: DbType.String,
                direction: ParameterDirection.ReturnValue,
                size: 4000);

            var affected = await conn.ExecuteAsync(
                sqlCmd.SqlCmdText,
                p,
                commandType:
                    sqlCmd.IsProcedure == "Y"
                    ? CommandType.StoredProcedure
                    : CommandType.Text);

            return Ok(new
            {
                isSuccess = true,
                recordsAffected = affected,
                returnValue = p.Get<object?>("return_value")
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
            p.Add("user_id", UserId);
        }

        if (request.ParentId != null)
        {
            p.Add("parent_id", request.ParentId);
        }

        return p;
    }
}