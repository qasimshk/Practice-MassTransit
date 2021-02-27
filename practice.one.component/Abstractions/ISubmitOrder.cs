using System;

namespace practice.one.component.Abstractions
{
    public interface ISubmitOrder
    {
        public Guid OrderRefrence { get; }
        public string ProductName { get; }
        public int Quantity { get; }
    }
}
