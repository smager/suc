namespace SmagerUp.Core.API.Models;

public class Account:LogColumns
{
    public Guid AccountId { get; set; }
    public string FirstName{ get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public bool IsLocked { get; set; }

}
