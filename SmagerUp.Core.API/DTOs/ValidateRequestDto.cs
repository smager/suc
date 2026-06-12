namespace SmagerUp.Core.API.DTOs;
public class ValidateRequestDto
{
    public Guid ClientId { get; set; }
    public string Key { get; set; } = string.Empty;
}
