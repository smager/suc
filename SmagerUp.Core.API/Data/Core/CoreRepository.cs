using Dapper;
using System.Data;

namespace SmagerUp.Core.API.Data.Core
{
    public class CoreRepository
    {
        private readonly CoreDapperContext _ctx;
        public CoreRepository(CoreDapperContext ctx) => _ctx = ctx;

        public async Task<Models.Core.Client?> GetClientByIdAsync(Guid ClientId)
        {
            using var con = _ctx.CreateConnection();
            return await con.QueryFirstOrDefaultAsync<Models.Core.Client>("Clients_sel", new { ClientId }, commandType:CommandType.StoredProcedure);
        }
    }
}
