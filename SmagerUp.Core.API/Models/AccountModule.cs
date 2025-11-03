namespace SmagerUp.Core.API.Models
{
    public class AccountModule
    {
        public Guid AccountModuleId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ModuleId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
