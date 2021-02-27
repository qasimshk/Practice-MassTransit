using MassTransit;
using MassTransit.MultiBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using practice.one.api.Configurations;
using practice.one.component.Abstractions;
using RabbitMQ.Client;
using System;
using System.Security.Authentication;

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
                cfg.AddRequestClient<ISubmitOrder>();

                cfg.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(options.AMQPURL);

                    cfg.ConfigureEndpoints(context);
                }));
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
    }
}
