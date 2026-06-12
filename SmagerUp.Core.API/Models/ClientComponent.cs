using System;

namespace SmagerUp.Core.API.Models
{
    public class ClientComponent : LogColumns
    {
        public Guid ClientComponentId{ get; set; } // keeps existing DB column name
        public Guid ClientId { get; set; }
        public Guid? ComponentId { get; set; }
        public DateTime? ExpiryAt { get; set; }
    }
}