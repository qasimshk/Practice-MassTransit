using System;

namespace practice.one.component.Abstractions
{
    public interface IOrderSubmitted
    {
        public Guid OrderRefrence { get; }
        public DateTime Timestamp { get; }
    }
}
