namespace SmagerUp.Core.API.DTOs.Admin;

public class ValidateRequestDto
{
    public Guid ClientId { get; set; }
    public string Key { get; set; } = string.Empty;
}

public class AdminUserValidateRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}