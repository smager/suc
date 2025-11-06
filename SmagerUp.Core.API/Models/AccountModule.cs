namespace SmagerUp.Core.API.Models
{
    public class AccountModule:LogColumns
    {
        public Guid AccountModuleId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ModuleId { get; set; }
        public DateTime? ExpiryAt { get; set; }
    }
}
