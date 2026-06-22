using System.Data;
using Microsoft.Data.SqlClient;

namespace SmagerUp.Core.API.Data.Core
{
    public class CoreDbContext
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public CoreDbContext(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("Default") 
                ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
