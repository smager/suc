namespace SmagerUp.Core.API.Models;

public class User : LogColumns
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
}

