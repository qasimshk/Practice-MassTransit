using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class SubmitOrderConsumer : IConsumer<ISubmitOrder>
    {
        public async Task Consume(ConsumeContext<ISubmitOrder> context)
        {
            if (context.Message.Quantity == 0)
            {
                await context.RespondAsync<IOrderRejected>(new OrderRejected
                {
                    OrderRefrence = context.Message.OrderRefrence,
                    Timestamp = InVar.Timestamp,
                    Reason = "Quantity is zero"
                });
            }
            else
            {
                await context.RespondAsync<IOrderSubmitted>(new OrderSubmitted
                {
                    OrderRefrence = context.Message.OrderRefrence,
                    Timestamp = InVar.Timestamp
                });
            }
        }
    }

    public class OrderRejected : IOrderRejected
    {
        public Guid OrderRefrence { get; set; }

        public DateTime Timestamp { get; set; }

        public string Reason { get; set; }
    }

    public class OrderSubmitted : IOrderSubmitted
    {
        public Guid OrderRefrence { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
