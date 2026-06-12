namespace SmagerUp.Core.API.Models.Client
{

    public class Client : LogColumns
    {
        public Guid ClientId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

    }
}
