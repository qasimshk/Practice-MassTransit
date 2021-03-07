using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class MemberNotification : IConsumer<INotifyMember>
    {
        public async Task Consume(ConsumeContext<INotifyMember> context)
        {
            await context.RespondAsync<IMemberNotified>(new MemberNotified
            {
                MemberId = context.Message.MemberId,
                Name = context.Message.Name,
                EmailAddress = context.Message.EmailAddress
            });
        }
    }

    public class MemberNotified : IMemberNotified
    {
        public Guid MemberId { get; init; }
        public string Name { get; init; }
        public string EmailAddress { get; init; }
    }
}
