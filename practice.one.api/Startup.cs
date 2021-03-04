using MassTransit;
using MassTransit.MultiBus;
using MassTransit.RabbitMqTransport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using practice.one.api.Configurations;
using practice.one.component.Abstractions;

namespace practice.one.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));

            var options = Configuration.GetSection(nameof(AppConfig)).Get<AppConfig>();

            services.AddMassTransit(cfg =>
            {
                cfg.ApplyCustomMassTransitConfiguration();

                //cfg.UsingRabbitMq((context, cfg) =>
                //{
                //    cfg.Host("amqp://cematix:Password123@cematixsrv:5672");

                //    MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
                //    MessageDataDefaults.Threshold = 2000;
                //    MessageDataDefaults.AlwaysWriteToRepository = false;
                //});

                cfg.AddRabbitMqMessageScheduler();
                
                cfg.UsingRabbitMq(ConfigureBus);

                //cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                //{
                //    //cfg.Host(options.AMQPURL);
                //    cfg.Host("amqp://cematix:Password123@cematixsrv:5672");
                //    cfg.ConfigureEndpoints(context);
                //}));

                cfg.AddRequestClient<ISubmitOrder>();
                cfg.AddRequestClient<OrderFry>();
            });

            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "practice.one.api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "practice.one.api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        static void ConfigureBus(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator configurator)
        {
            //configurator.Host("amqp://cematix:Password123@cematixsrv:5672");
            configurator.Host("");

            //MessageDataDefaults.ExtraTimeToLive = TimeSpan.FromDays(1);
            //MessageDataDefaults.Threshold = 2000;
            //MessageDataDefaults.AlwaysWriteToRepository = false;
            
            /*
            "RabbitMQ": {
                "Host": "rabbitmq://192.168.160.131/",
                "Username": "test",
                "Password": "1"
            },
            configurator.Host(new Uri(""), host =>
            {
                host.Username("");
                host.Password("");


            });
            */
            configurator.ConfigureEndpoints(context);
        }
    }
}
