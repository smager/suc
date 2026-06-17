namespace SmagerUp.Core.API.Models
{
    public class SqlCommandInfo:LogColumns
    {
        public Guid SqlCmdId { get; set; }

        public string SqlCmdCode { get; set; } = "";

        public string SqlCmdText { get; set; } = "";

        public string IsProcedure { get; set; } = "N";

        public string IsPublic { get; set; } = "N";


    }
}
