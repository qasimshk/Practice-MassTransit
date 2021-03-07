using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using practice.one.service.StateMachine;

namespace practice.one.service.Persistance
{
    public class MemberStateMap : SagaClassMap<MemberState>
    {
        protected override void Configure(EntityTypeBuilder<MemberState> entity, ModelBuilder model)
        {
            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.CreatedOn);
            entity.Property(x => x.FailedOn);
        }
    }
}
