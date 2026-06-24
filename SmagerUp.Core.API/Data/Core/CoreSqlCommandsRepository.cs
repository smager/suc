using Dapper;
using SmagerUp.Core.API.Data.Core;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data.Client
{
    public class CoreSqlCommandsRepository
    {
        private readonly CoreDbContext _ctx;
        public CoreSqlCommandsRepository(CoreDbContext ctx) => _ctx = ctx;

       

        public async Task<SqlCommandInfo?> GetByCodeAsync(string sqlCode)
        {
            using var con = _ctx.CreateConnection();

            return await con.QueryFirstOrDefaultAsync<SqlCommandInfo>("SqlCommands_sel", new { SqlCode = sqlCode },commandType:System.Data.CommandType.StoredProcedure);
        }


    }
}
