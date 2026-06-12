namespace SmagerUp.Core.API.DTOs.Core;
public class ValidateRequestDto
{
    public Guid ClientId { get; set; }
    public string Key { get; set; } = string.Empty;
}
