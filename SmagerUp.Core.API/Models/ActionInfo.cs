namespace SmagerUp.Core.API.Models;

public class ActionInfo:LogColumns
{
    public Guid ActionId { get; set; }

    public string ActionCode { get; set; } = "";
    public string ActionType { get; set; } = "";
    public string CommandText { get; set; } = "";
    public string CommandType { get; set; } = "";
    public string IsPublic { get; set; } = "N";


}
