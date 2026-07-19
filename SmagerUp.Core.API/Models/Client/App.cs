namespace SmagerUp.Core.API.Models.Client; 
public class App : LogColumns {
    public Guid AppId { get; set; }
    public Guid ClientId { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string ConnStr { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;

}

