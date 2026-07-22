using Microsoft.Data.SqlClient;
using SmagerUp.Core.API.Services;
using System.Data;

namespace SmagerUp.Core.API.Data.Admin;

public class AdminDbContext
{
    private readonly IConfiguration _config;
    private readonly IEncryptionService _encryption;
    private readonly string _connectionString;

    public AdminDbContext(IConfiguration config,IEncryptionService encryption)
    {

 

        _encryption= encryption;
        _config = config;
        _connectionString = _encryption.Decrypt(_config.GetConnectionString("Default")) 
            ?? throw new InvalidOperationException("Connection string 'Default' not found.");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection( _connectionString);
}
