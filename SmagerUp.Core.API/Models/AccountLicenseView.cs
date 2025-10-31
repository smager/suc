namespace SmagerUp.Core.API.Models
{
    public class AccountLicenseView
    {
        public Guid AccountLicenseId { get; set; }
        public string LicenseTypeName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? BodyContent { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }

}
