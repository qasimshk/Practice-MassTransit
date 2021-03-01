using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using System;
using GreenPipes;
using practice.one.component.Abstractions;

namespace practice.one.service.Consumers
{
    public class MessageNotifyConsumerDefinition : ConsumerDefinition<MessageNotifyConsumer>
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageNotifyConsumerDefinition(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            ConcurrentMessageLimit = 20;
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MessageNotifyConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
            endpointConfigurator.UseServiceScope(_serviceProvider);

            consumerConfigurator.Message<IMessageNotify>(m => m.UseFilter(new ContainerScopedFilter()));

            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}
