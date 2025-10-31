namespace SmagerUp.Core.API.Models
{
    public class AccountLicense
    {
        public Guid AccountLicenseId { get; set; }
        public Guid AccountId { get; set; }
        public int LicenseTypeId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
