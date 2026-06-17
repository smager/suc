namespace SmagerUp.Core.API.DTOs
{
    public class DataRequest
    {
        public string? SqlCode { get; set; }
        public string? Procedure { get; set; }
        public Dictionary<string, object>? Parameters { get; set; }
        public List<Dictionary<string, object>>? Rows { get; set; }
        public object? ParentId { get; set; }
    }
}
