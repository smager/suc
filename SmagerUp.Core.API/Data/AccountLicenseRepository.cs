using Dapper;
using SmagerUp.Core.API.Models;

namespace SmagerUp.Core.API.Data
{
    public class AccountLicenseRepository
    {
        private readonly DapperContext _db;

        public AccountLicenseRepository(DapperContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<AccountLicenseView>> GetLicensesForAccount(Guid accountId)
        {
            const string sql = @"
                SELECT 
                    AL.AccountLicenseId,
                    LT.Name LicenseTypeName,
                    LT.Type,
                    LT.BodyContent,
                    AL.ExpiryDate,
                    AL.IsActive
                FROM AccountLicenses AL
                INNER JOIN LicenseTypes LT ON AL.LicenseTypeId = LT.LicenseTypeId
                WHERE AL.AccountId = @accountId
                ORDER BY LT.Name;
            ";

            using var connection = _db.CreateConnection();
            return await connection.QueryAsync<AccountLicenseView>(sql, new { accountId });
        }

        public async Task<int> AssignLicenseAsync(Guid accountId, int licenseTypeId, DateTime? expiryDate)
        {
            const string sql = @"
                INSERT INTO AccountLicenses (AccountId, LicenseTypeId, ExpiryDate, IsActive)
                VALUES (@accountId, @licenseTypeId, @expiryDate, 1);
            ";

            using var connection = _db.CreateConnection();
            return await connection.ExecuteAsync(sql, new { accountId, licenseTypeId, expiryDate });
        }

        public async Task<int> DeactivateLicenseAsync(Guid accountLicenseId)
        {
            const string sql = "UPDATE AccountLicenses SET IsActive = 0 WHERE AccountLicenseId = @accountLicenseId";
            using var connection = _db.CreateConnection();
            return await connection.ExecuteAsync(sql, new { accountLicenseId });
        }
    }
}
