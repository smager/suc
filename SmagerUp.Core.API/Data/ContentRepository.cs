using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data
{
    public class ContentRepository
    {
        private readonly DapperContext _db;
        public ContentRepository(DapperContext db) => _db = db;

        public async Task<IEnumerable<Content>> GetContentsByGroupAsync(Guid contentGroupId)
        {
            const string sql = @"
                SELECT c.ContentId, c.ContentBody, c.ContentType, c.CreatedAt
                FROM Contents c
                INNER JOIN ContentGroups cg ON c.ContentId = cg.ContentId
                WHERE cg.ContentGroupId = @groupId AND c.IsDeleted = 0 
                ORDER BY c.ContentType;";

            using var conn = _db.CreateConnection();
            var result = await conn.QueryAsync<Content>(sql, new { groupId = contentGroupId });
            return result;
        }
    }
}
