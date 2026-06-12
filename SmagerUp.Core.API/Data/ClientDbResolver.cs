using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

public interface IClientDbResolver
{
    IDbConnection CreateConnection();
}

namespace SmagerUp.Core.API.Data
{
    public class ClientDbResolver : IClientDbResolver
    {
        private readonly IHttpContextAccessor _http;
        private readonly CoreDapperContext _core;

        public ClientDbResolver(
            IHttpContextAccessor http,
            CoreDapperContext core)
        {
            _http = http;
            _core = core;
        }

        public IDbConnection CreateConnection()
        {
            var clientId = GetClientId();

            using var coreConn = _core.CreateConnection();

            var cs = coreConn.QuerySingle<string>(
                "SELECT ConnectionString FROM Clients WHERE ClientId=@id",
                new { id = clientId });

            return new SqlConnection(cs);
        }

        private int GetClientId()
        {
            var value = _http.HttpContext?
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
    }
}
