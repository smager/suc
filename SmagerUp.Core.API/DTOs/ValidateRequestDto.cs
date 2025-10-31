namespace SmagerUp.Core.API.DTOs;
public class ValidateRequestDto
{
    public Guid AccountId { get; set; }
    public string Key { get; set; } = string.Empty;
}
