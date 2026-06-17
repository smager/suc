using Dapper;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.Models;
using SmagerUp.Core.API.Models.Core;

namespace SmagerUp.Core.API.Data.Client
{
    public class SqlCommandsRepository
    {
        private readonly IClientDbResolver _db;

        public SqlCommandsRepository(   IClientDbResolver db)
        {
            _db = db;
        }

        public async Task<SqlCommandInfo?> GetByCodeAsync(Guid clientId, string sqlCode)
        {
            const string sql = @"
              SELECT SqlcmdCode,SqlcmdText,IsProcedure,IsPublic FROM su_SqlCommands 
              WHERE SqlCode = @SqlCode
              AND IsActive = 1;";

            using var conn =_db.CreateConnection(clientId);

            return await conn.QueryFirstOrDefaultAsync<SqlCommandInfo>(
                sql,
                new { SqlCode = sqlCode });
        }
    }
}
