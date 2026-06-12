using Dapper;
using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Data.Core;
using System.Data;

namespace SmagerUp.Core.API.Data.Client
{
    public class ClientDapperContext
    {
        private readonly string _connectionString;

        public ClientDapperContext(
            IHttpContextAccessor httpContext,
            CoreDapperContext coreDb)
        {
            var clientId = GetClientIdFromToken(httpContext);

            using var conn = coreDb.CreateConnection();

            _connectionString =
                conn.QuerySingle<string>(
                    "SELECT ConnectionString FROM Clients WHERE ClientId=@id",
                    new { id = clientId });
        }
        private int GetClientIdFromToken(IHttpContextAccessor httpContext)
        {
            var value = httpContext.HttpContext?
                .User
                .FindFirst("ClientId")?
                .Value;

            if (!int.TryParse(value, out var clientId))
            {
                throw new UnauthorizedAccessException(
                    "ClientId claim not found.");
            }

            return clientId;
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }

}
