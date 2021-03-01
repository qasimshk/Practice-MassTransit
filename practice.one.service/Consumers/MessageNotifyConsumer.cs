using MassTransit;
using practice.one.component.Abstractions;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class MessageNotifyConsumer : IConsumer<IMessageNotify>
    {
        public Task Consume(ConsumeContext<IMessageNotify> context)
        {
            string Name = context.Message.Name;

            return Task.CompletedTask;
        }
    }
}
