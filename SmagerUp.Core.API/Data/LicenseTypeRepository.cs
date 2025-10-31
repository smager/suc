using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data
{
    public class LicenseTypeRepository
    {
        private readonly DapperContext _db;

        public LicenseTypeRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LicenseType>> GetAllAsync()
        {
            const string sql = "SELECT * FROM LicenseTypes ORDER BY CreatedAt DESC";
            using var connection = _db.CreateConnection();
            return await connection.QueryAsync<LicenseType>(sql);
        }

        public async Task<LicenseType?> GetByIdAsync(int licenseTypeId)
        {
            const string sql = "SELECT * FROM LicenseTypes WHERE LicenseTypeId = @licenseTypeId";
            using var connection = _db.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<LicenseType>(sql, new { licenseTypeId });
        }

        public async Task<int> CreateAsync(LicenseType license)
        {
            const string sql = @"
                INSERT INTO LicenseTypes (Name, Type, BodyContent)
                VALUES (@Name, @Type, @BodyContent);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            using var connection = _db.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, license);
        }

        public async Task<int> UpdateAsync(LicenseType license)
        {
            const string sql = @"
                UPDATE LicenseTypes
                SET Name = @Name,
                    Type = @Type,
                    BodyContent = @BodyContent
                WHERE LicenseTypeId = @LicenseTypeId;
            ";
            using var connection = _db.CreateConnection();
            return await connection.ExecuteAsync(sql, license);
        }

        public async Task<int> DeleteAsync(int licenseTypeId)
        {
            const string sql = "DELETE FROM LicenseTypes WHERE LicenseTypeId = @licenseTypeId";
            using var connection = _db.CreateConnection();
            return await connection.ExecuteAsync(sql, new { licenseTypeId });
        }
    }
}
