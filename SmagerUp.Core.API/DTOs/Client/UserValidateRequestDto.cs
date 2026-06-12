namespace SmagerUp.Core.API.DTOs.Client;
public class UserValidateRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ClientUserValidateRequestDto
{
    public Guid ClientId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
