namespace SmagerUp.Core.API.DTOs; 
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = "";
}


public class TokenResponseDto
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}