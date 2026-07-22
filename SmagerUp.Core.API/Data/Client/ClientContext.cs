using Dapper;
using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Data.Admin;
using SmagerUp.Core.API.Services;
using System.Data;

namespace SmagerUp.Core.API.Data.Client;

public class ClientDbContext : IClientDbResolver
{
    private readonly ILogger<ClientDbContext> _log;
    private readonly AdminDbContext _core;

    public ClientDbContext(ILogger<ClientDbContext> log, IHttpContextAccessor http, AdminDbContext core)
    {
        _log = log; 
        _core = core; 
    }

    public IDbConnection CreateConnection(string ApiKey)
    {
        using var coreConn = _core.CreateConnection();

        var cs = coreConn.QuerySingleOrDefault<string>(
            "SELECT dbo.GetConnectionString(@ApiKey)", new { ApiKey });

        var _crypt = new Cryptography();
        var decryptedCS = _crypt.Decrypt(cs);  
        _crypt=null;

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException($"Connection string not found for ApiKey {ApiKey}");

        _log.LogInformation("Resolved client connection string: {cs}", cs);
        // optional quick test
        /*
        try
        {
            using var test = new SqlConnection(cs);
            test.Open();
            test.Close();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Failed to open client DB from resolved connection string");
            throw;
        }
        */
        return new SqlConnection(decryptedCS);
    }

}
