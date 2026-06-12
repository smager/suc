using Dapper;
using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Data.Core;
using System.Data;
using System.Text.RegularExpressions;

public interface IClientDbResolver
{
    IDbConnection CreateConnection(Guid? ClientId);
}

namespace SmagerUp.Core.API.Data.Client
{
    
    public class ClientDbResolver : IClientDbResolver
    {
        private readonly ILogger<ClientDbResolver> _log;
        private readonly CoreDapperContext _core;
        public ClientDbResolver(ILogger<ClientDbResolver> log, IHttpContextAccessor http, CoreDapperContext core)
        {
            _log = log; 
            _core = core; 
        }

        public IDbConnection CreateConnection(Guid? ClientId)
        {
            using var coreConn = _core.CreateConnection();
            var cs = coreConn.QuerySingleOrDefault<string>(
                "SELECT ConnectionString FROM Clients WHERE ClientId=@id",
                new { id = ClientId });

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException($"Connection string not found for client {ClientId}");

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
            return new SqlConnection(cs);
        }

    }
}
