namespace SmagerUp.Core.API.Models.Admin;

public class Client : LogColumns {
    public Guid ClientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

}


