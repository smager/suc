using System;
using System.Collections.Generic;

namespace SmagerUp.Core.API.DTOs
{
    public class ResourceDto
    {
        public string Type { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class ComponentResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public IEnumerable<ResourceDto> Resources { get; set; } = Array.Empty<ResourceDto>();
    }
}