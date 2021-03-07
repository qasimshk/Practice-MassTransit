using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace practice.one.service.Persistance
{
    public class MemberStateDbContext : SagaDbContext
    {
        public MemberStateDbContext(DbContextOptions option) : base(option) { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new MemberStateMap(); }
        }
        
    }
}
