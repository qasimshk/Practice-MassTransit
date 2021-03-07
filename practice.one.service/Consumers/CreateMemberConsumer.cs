using MassTransit;
using practice.one.component.Abstractions;
using System;
using System.Threading.Tasks;

namespace practice.one.service.Consumers
{
    public class CreateMemberConsumer : IConsumer<IRegisterMember>
    {
        public async Task Consume(ConsumeContext<IRegisterMember> context)
        {
            if (context.Message.EmailAddress == "test@test.com")
            {
                await context.RespondAsync<IMemberFailed>(new MemberFailed
                {
                    MemberId = context.Message.MemberId,
                    Name = context.Message.Name,
                    ErrorMessage = "this is a test email",
                    EmailAddress = context.Message.EmailAddress
                });
            }
            else
            {
                await context.RespondAsync<IMemberAdded>(new MemberAdded
                {
                    MemberId = context.Message.MemberId,
                    Name = context.Message.Name,
                    EmailAddress = context.Message.EmailAddress
                });
            }
        }
    }

    public class MemberFailed : IMemberFailed
    {
        public Guid MemberId { get; init; }
        public string Name { get; init; }
        public string ErrorMessage { get; init; }
        public string EmailAddress { get; init; }
    }

    public class MemberAdded : IMemberAdded
    {
        public Guid MemberId { get; init; }
        public string Name { get; init; }
        public string EmailAddress { get; init; }
    }
}

