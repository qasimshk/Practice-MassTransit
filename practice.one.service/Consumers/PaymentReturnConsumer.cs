using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class PaymentReturnConsumer : IConsumer<IPaymentReturn>
    {
        public Task Consume(ConsumeContext<IPaymentReturn> context)
        {
            if (context.Message.Amount == 0)
            {
                throw new InvalidOperationException("amount is zero");
            }

            return Task.CompletedTask;
        }
    }

    
}
