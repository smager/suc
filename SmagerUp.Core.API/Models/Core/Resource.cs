using System;

namespace SmagerUp.Core.API.Models.Core
{
    public class Resource : LogColumns
    {
        public Guid ResourceId { get; set; }
        public string ResourceName { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty; // e.g. "js","css","html"
        public string ResourceBody { get; set; } = string.Empty;
        public int? Version { get; set; }
    }
}