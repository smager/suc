namespace SmagerUp.Core.API.Models
{
    public class Content
    {
        public Guid ContentId { get; set; }
        public string ContentBody { get; set; } = string.Empty; // store html/css/js/text
        public string ContentType { get; set; } = string.Empty; // "js", "css", "html", "txt"
        public DateTime CreatedAt { get; set; }
    }
}
