namespace SmagerUp.Core.API.Models
{
    public class Content:LogColumns
    {
        public Guid ContentId { get; set; }
        public string ContentName { get; set; } = string.Empty;
        public string Title {  get; set; }= string.Empty;
        public string Body { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty; 
    }
}
