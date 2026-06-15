namespace SmagerUp.Core.API.Models.Client
{

    public class User : LogColumns
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
        public bool IsLocked { get; set; }

    }

    //public class User2
    //{
    //   //public Guid UserId { get; set; }
    //    public string UserName { get; set; } = string.Empty;
    //    public string Password { get; set; } = string.Empty;

    //}
}
