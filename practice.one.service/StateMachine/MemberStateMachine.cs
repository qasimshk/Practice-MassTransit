using Automatonymous;
using practice.one.component.Abstractions;
using System;

namespace practice.one.service.StateMachine
{
    public class MemberStateMachine : MassTransitStateMachine<MemberState>
    {
        public MemberStateMachine()
        {
            Event(() => MemberAdded, x => x.CorrelateById(m => m.Message.MemberId));

            Event(() => MemberFailed, x => x.CorrelateById(m => m.Message.MemberId));

            Event(() => MemberNotified, x => x.CorrelateById(m => m.Message.MemberId));


            InstanceState(s => s.CurrentState);

            Initially(
                When(MemberAdded)
                    .Then(context =>
                    {
                        context.Instance.MemberId = context.Data.MemberId;
                        context.Instance.Name = context.Data.Name;
                        context.Instance.EmailAddress = context.Data.EmailAddress;
                    })
                    .TransitionTo(MemberCreated)
                    .Publish(context => new NotifyMemberRequest(context.Instance)),
                When(MemberFailed)
                    .Then(context =>
                    {
                        context.Instance.FailedOn = DateTime.Now;
                        context.Instance.MemberId = context.Data.MemberId;
                        context.Instance.Name = context.Data.Name;
                        context.Instance.EmailAddress = context.Data.EmailAddress;
                        context.Instance.ErrorMessage = context.Data.ErrorMessage;
                    })
                    .TransitionTo(MemberCreatingFailed)
            );

            During(MemberCreated,
                When(MemberNotified)
                    .Then(context => context.Instance.CreatedOn = DateTime.Now)
                    .TransitionTo(MemberCreatedAndNotified)
            );

            SetCompletedWhenFinalized();
        }

        public State MemberCreated { get; private set; }
        public State MemberCreatingFailed { get; private set; }
        public State MemberCreatedAndNotified { get; private set; }


        public Event<IMemberAdded> MemberAdded { get; private set; }
        public Event<IMemberFailed> MemberFailed { get; private set; }
        public Event<IMemberNotified> MemberNotified { get; private set; }
    }

    public class NotifyMemberRequest : INotifyMember
    {
        private readonly MemberState _memberState;

        public NotifyMemberRequest(MemberState memberState)
        {
            _memberState = memberState;
        }

        public Guid MemberId => _memberState.MemberId;
        public string EmailAddress => _memberState.EmailAddress;
        public string Name => _memberState.Name;
    }
}
