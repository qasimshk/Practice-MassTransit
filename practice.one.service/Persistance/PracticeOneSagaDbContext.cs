using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace practice.one.api.Persistance
{
    public class PracticeOneSagaDbContext : SagaDbContext
    {
        public PracticeOneSagaDbContext(DbContextOptions options) : base(options) { }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new FutureStateMap(); }
        }
    }
}
