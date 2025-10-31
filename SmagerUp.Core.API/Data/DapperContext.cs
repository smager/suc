using System.Data;
using Microsoft.Data.SqlClient;

namespace SmagerUp.Core.API.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DapperContext(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("Default") 
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
