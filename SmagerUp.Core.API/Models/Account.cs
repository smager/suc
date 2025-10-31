namespace SmagerUp.Core.API.Models;

public class Account
{
    public Guid AccountId { get; set; }
    public string FirstName{ get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
