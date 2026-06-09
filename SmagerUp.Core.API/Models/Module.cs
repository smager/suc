namespace SmagerUp.Core.API.Models
{
    public class Module:LogColumns
    {
        public Guid ModuleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public Guid LicenseTypeId { get; set; }
        public decimal Price { get; set; }
        public Guid? ContentGroupId { get; set; } // new FK
    }
}
