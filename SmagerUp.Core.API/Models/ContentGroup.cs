namespace SmagerUp.Core.API.Models
{
    public class ContentGroup
    {
        public Guid ContentGroupId { get; set; }
        public Guid ContentId { get; set; } // FK to Content.ContentId (could be normalized differently)
        public DateTime CreatedAt { get; set; }
    }
}
