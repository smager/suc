namespace SmagerUp.Core.API.Models
{
    public class LicenseType
    {
        public int LicenseTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? BodyContent { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }

}
