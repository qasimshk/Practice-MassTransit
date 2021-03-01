using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace practice.one.service
{
    public class Worker : IHostedService //BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl _bus;

        public Worker(ILogger<Worker> logger, IBusControl bus)
        {
            _logger = logger;
            _bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync(cancellationToken);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync(cancellationToken);
            _logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
        }

        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    await _bus.StartAsync(stoppingToken).ConfigureAwait(false);

        //    //while (!stoppingToken.IsCancellationRequested)
        //    //{
        //    //    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //    //    await Task.Delay(1000, stoppingToken);
        //    //}
        //}

        
    }
}
