namespace SmagerUp.Core.API.DTOs.Client;

public class ClientUserValidateRequestDto
{
    public Guid ClientId { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = "";
}


public class TokenResponseDto
{
    public bool IsHostAccess { get; set; } 
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}