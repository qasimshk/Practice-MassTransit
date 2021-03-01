using System;

namespace practice.one.component.Abstractions
{
    public interface IPaymentReturn
    {
        public Guid OrderId { get; }
        public int Amount { get; set; }
    }
}
