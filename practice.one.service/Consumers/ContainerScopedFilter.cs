using GreenPipes;
using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    class ContainerScopedFilter : IFilter<ConsumeContext<IMessageNotify>>
    {
        public void Probe(ProbeContext context)
        {
            Console.WriteLine("Filter ran - Probe");
        }

        public Task Send(ConsumeContext<IMessageNotify> context, IPipe<ConsumeContext<IMessageNotify>> next)
        {
            var provider = context.GetPayload<IServiceProvider>();

            Console.WriteLine("Filter ran - Send");

            return next.Send(context);
        }
    }
}
