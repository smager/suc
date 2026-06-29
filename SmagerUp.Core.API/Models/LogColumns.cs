namespace SmagerUp.Core.API.Models;

public class LogColumns
{
    public bool IsDeleted {  get; set; }
    public bool IsActive {  get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid UpdatedBy { get; set; }
    public Guid DeletedBy { get; set; }
}
