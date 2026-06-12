using System;

namespace SmagerUp.Core.API.Models
{
    public class ComponentResource : LogColumns
    {
        public Guid ComponentResourceId { get; set; }
        public Guid ComponentId { get; set; }
        public Guid ResourceId { get; set; }
    }
}