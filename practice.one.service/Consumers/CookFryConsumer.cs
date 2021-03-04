using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class CookFryConsumer : IConsumer<CookFry>
    {
        public async Task Consume(ConsumeContext<CookFry> context)
        {
            await context.RespondAsync<FryReady>(new FryResponse { 
                
                OrderRefrence = context.Message.OrderRefrence,
                ProductName = $"{context.Message.ProductName} AYA",
                Quantity = context.Message.Quantity
            });
        }
    }

    public class FryResponse : FryReady
    {
        public Guid OrderId { get; set; }

        public Guid OrderLineId { get; set; }

        public Guid OrderRefrence { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public DateTime Created { get; set; }

        public DateTime Completed { get; set; }
    }
}
