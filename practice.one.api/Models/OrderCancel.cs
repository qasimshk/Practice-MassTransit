using System;

namespace practice.one.api.Models
{
    public class OrderCancel 
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }
}
