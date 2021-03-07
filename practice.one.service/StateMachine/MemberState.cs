using Automatonymous;
using System;

namespace practice.one.service.StateMachine
{
    public class MemberState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? FailedOn { get; set; }
        public Guid MemberId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
