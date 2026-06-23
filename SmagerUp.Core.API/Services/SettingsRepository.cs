using Dapper;
using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Controllers.Core;
using System.Data;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Services
{
    public class SettingsRepository
    {
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryption;

        public SettingsRepository(IConfiguration config, IEncryptionService encryption)
        {
            _config = config;
            _encryption = encryption;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(_encryption.Decrypt(_config.GetConnectionString("Default") ));
        }

        public async Task<Dictionary<string, string>> GetCategoryAsync(string category)
        {
            using var con = GetConnection();

            var items = await con.QueryAsync<Setting>(@"SELECT ConfigKey, Value FROM Settings WHERE Category = @Category", new { Category = category });

            return items.ToDictionary( x => x.ConfigKey,x => x.Value ?? "");
        }
    }
}
