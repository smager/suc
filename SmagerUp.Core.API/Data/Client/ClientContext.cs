using Dapper;
using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.Models.Core;
using SmagerUp.Core.API.Services;
using System.Data;
using System.Text.RegularExpressions;

public interface IClientDbResolver
{
    IDbConnection CreateConnection(Guid ClientId);
}

namespace SmagerUp.Core.API.Data.Client
{
    
    public class ClientDbContext : IClientDbResolver
    {
        private readonly ILogger<ClientDbContext> _log;
        private readonly CoreDbContext _core;
        private readonly IEncryptionService _encryption;

        public ClientDbContext(ILogger<ClientDbContext> log, IHttpContextAccessor http, CoreDbContext core, IEncryptionService encryption   )
        {
            _log = log; 
            _core = core; 
            _encryption = encryption;
        }

        public IDbConnection CreateConnection(Guid ClientId)
        {
            using var coreConn = _core.CreateConnection();

            var cs = coreConn.QuerySingleOrDefault<string>(
                "SELECT dbo.GetConnectionString(@ClientId)", new { ClientId });

            var decryptedCS = _encryption.Decrypt(cs);   

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
            return new SqlConnection(decryptedCS);
        }

    }
}
