using MassTransit;
using System;

namespace practice.one.component.Abstractions
{
    public interface IRegisterMember
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
    }

    public interface IMemberRegistrationSuccessful : CorrelatedBy<Guid>
    {
        public Guid MemberId { get; }
        string CurrentState { get; }
    }

    public interface IMemberRegistrationFailed : CorrelatedBy<Guid>
    {
        public Guid MemberId { get; }
        string State { get; }
        public string ErrorMessage { get; }
    }


    public interface IAddMember : CorrelatedBy<Guid>
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
    }

    public interface IMemberAdded
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
    }

    public interface IMemberFailed
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
        public string ErrorMessage { get; }
    }

    public interface INotifyMember
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
    }

    public interface IMemberNotified
    {
        public Guid MemberId { get; }
        public string Name { get; }
        public string EmailAddress { get; }
    }
}
