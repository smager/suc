namespace SmagerUp.Core.API.DTOs
{
    public class DataRequest
    {
        public string? ActionCode { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public List<Dictionary<string, object>>? Rows { get; set; }
    }
}
