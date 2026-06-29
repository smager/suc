namespace SmagerUp.Core.API.Models;

public class EmailSettings
{
    public string Address { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Password { get; set; } = "";
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public bool IsSSL { get; set; }
}
