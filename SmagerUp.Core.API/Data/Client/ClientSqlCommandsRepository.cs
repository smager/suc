using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data.Client
{
    public class ClientSqlCommandsRepository
    {
        private readonly IClientDbResolver _db;

        public ClientSqlCommandsRepository(   IClientDbResolver db)
        {
            _db = db;
        }

        public async Task<SqlCommandInfo?> GetByCodeAsync(Guid clientId, string sqlCode)
        {

            using var conn =_db.CreateConnection(clientId);

            return await conn.QueryFirstOrDefaultAsync<SqlCommandInfo>("dbo.su_SqlCommands_sel", new { SqlCode = sqlCode },commandType:System.Data.CommandType.StoredProcedure);
        }


    }
}
