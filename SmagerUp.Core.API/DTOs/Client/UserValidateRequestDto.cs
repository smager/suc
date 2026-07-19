namespace SmagerUp.Core.API.DTOs.Client;

public class AppValidateRequestDto
{
    public string ApiKey { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


