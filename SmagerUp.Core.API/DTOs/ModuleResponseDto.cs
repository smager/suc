namespace SmagerUp.Core.API.DTOs
{
    public class ModuleContentDto
    {
        public string Type { get; set; } = string.Empty; // "js", "css", "html"
        public string Body { get; set; } = string.Empty;
    }

    public class ModuleResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IEnumerable<ModuleContentDto> Contents { get; set; } = Array.Empty<ModuleContentDto>();
        //public string Account { get; set; } = string.Empty; // account name requesting it
    }
}
