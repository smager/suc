using System;

namespace SmagerUp.Core.API.Models.Core
{
    public class Component : LogColumns
    {
        public Guid ComponentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public Guid ComponentTypeId { get; set; }
        public Guid? LicenseTypeId { get; set; }
        public decimal Price { get; set; }
    }
}
