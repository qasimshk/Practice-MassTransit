using MassTransit;
using MassTransit.Definition;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using practice.one.component.Activities;
using practice.one.service.Activities;
using practice.one.service.Configuration;
using practice.one.service.Consumers;
using practice.one.service.Persistance;
using practice.one.service.StateMachine;
using System;
using System.Reflection;

namespace practice.one.service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<AppConfig>(options => hostContext.Configuration.GetSection("AppConfig").Bind(options));
                    
                    string connectionstring =
                        "Data Source=tcp:cematixsrv;Initial Catalog=PracticeDb;User Id=sa;Password=Password123; MultipleActiveResultSets=true";
                    
                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddRabbitMqMessageScheduler();

                        // Saga

                        cfg.AddSagaStateMachine<MemberStateMachine, MemberState>()
                            .EntityFrameworkRepository(ef =>
                            {
                                ef.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                                ef.AddDbContext<DbContext, MemberStateDbContext>((provider, builder) =>
                                {
                                    builder.UseSqlServer(connectionstring, m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(MemberStateDbContext)}");
                                    });
                                });
                            });
                        
                        // Saga

                        cfg.AddConsumersFromNamespaceContaining<MessageNotifyConsumer>();

                        cfg.AddActivitiesFromNamespaceContaining<OrderCancelledActivity>();
                        
                        cfg.AddExecuteActivity<ProcessingOrderCancelActivity, OrderCancelledArguements>();
                        
                        cfg.AddRabbitMqMessageScheduler();

                        cfg.UsingRabbitMq(ConfigureBus);
                    });

                    services.AddHostedService<Worker>();
                });

        static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host("");
            
            configurator.ConfigureEndpoints(context);
        }
    }
}
