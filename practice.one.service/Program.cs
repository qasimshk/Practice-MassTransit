using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.Futures;
using MassTransit.RabbitMqTransport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using practice.one.api.Persistance;
using practice.one.component.Activities;
using practice.one.service.Activities;
using practice.one.service.Configuration;
using practice.one.service.Consumers;
using practice.one.service.Futures;
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

                    // Saga
                    services.AddDbContext<PracticeOneSagaDbContext>(builder =>
                        builder.UseSqlServer("Data Source=DESKTOP-KM44QRM;Initial Catalog=PracticeDb;User Id=sa;Password=123; MultipleActiveResultSets=true", m =>
                        {
                            m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                            m.MigrationsHistoryTable($"__{nameof(PracticeOneSagaDbContext)}");
                        }));
                    // Saga

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddRabbitMqMessageScheduler();

                        // Saga
                        cfg.SetEntityFrameworkSagaRepositoryProvider(r =>
                        {
                            r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                            r.LockStatementProvider = new SqlServerLockStatementProvider();

                            r.ExistingDbContext<PracticeOneSagaDbContext>();
                        });
                        // Saga

                        cfg.AddConsumersFromNamespaceContaining<MessageNotifyConsumer>();

                        cfg.AddActivitiesFromNamespaceContaining<OrderCancelledActivity>();

                        cfg.AddFuturesFromNamespaceContaining<OrderFuture>();

                        // Saga
                        cfg.AddSagaRepository<FutureState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = ConcurrencyMode.Pessimistic;
                                r.LockStatementProvider = new SqlServerLockStatementProvider();

                                r.ExistingDbContext<PracticeOneSagaDbContext>();
                            });
                        // Saga

                        cfg.AddExecuteActivity<ProcessingOrderCancelActivity, OrderCancelledArguements>();

                        //cfg.AddConsumers(Assembly.GetExecutingAssembly());

                        //cfg.AddServiceClient();
                        //cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        //{
                        //    //cfg.Host("amqps://fipkceeo:xP_9YsvpoPXNcc1uQ8LgrvMwZfJZrMgF@rattlesnake.rmq.cloudamqp.com/fipkceeo");
                        //    cfg.Host("amqp://cematix:Password123@cematixsrv:5672");

                        //    cfg.ReceiveEndpoint(nameof(IMessageNotify), ep =>
                        //    {
                        //        ep.Consumer<MessageNotifyConsumer>();

                        //        ep.PrefetchCount = 16;
                        //        ep.UseMessageRetry(r => r.Interval(2, 100));
                        //        ep.ConfigureConsumers(context);
                        //    });

                        //    cfg.ConfigureEndpoints(context);

                        //}));

                        cfg.AddRabbitMqMessageScheduler();

                        cfg.UsingRabbitMq(ConfigureBus);
                    });

                    services.AddHostedService<Worker>();
                });

        static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.Host("");

            //MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
            //MessageDataDefaults.Threshold = 2000;
            //MessageDataDefaults.AlwaysWriteToRepository = false;

            configurator.ConfigureEndpoints(context);
        }
    }
}
