using practice.one.component.Abstractions;
using System;

namespace practice.one.api.Models
{
    public class SubmitOrder : ISubmitOrder
    {        
        public Guid OrderRefrence { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
