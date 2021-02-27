using System;

namespace practice.one.component.Abstractions
{
    public interface IOrderRejected
    {
        public Guid OrderRefrence { get; }
        public DateTime Timestamp { get; }
        public string Reason { get; }
    }
}
