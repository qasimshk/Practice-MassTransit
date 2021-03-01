using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace practice.one.service.Consumers
{
    public class RoutingSlipConsumer : IConsumer<RoutingSlipCompleted>, IConsumer<RoutingSlipActivityCompleted>, IConsumer<RoutingSlipFaulted>
    {
        private readonly ILogger<RoutingSlipConsumer> _logger;

        public RoutingSlipConsumer(ILogger<RoutingSlipConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            _logger.LogInformation($"RoutingSlip completed - Tracking number {context.Message.TrackingNumber}");

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            _logger.LogInformation($"RoutingSlip activity completed - Tracking number {context.Message.TrackingNumber} - activity name {context.Message.ActivityName}");

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            _logger.LogInformation($"RoutingSlip activity faulted - Tracking number {context.Message.TrackingNumber} - activity name {context.Message.ActivityExceptions.FirstOrDefault()}");

            return Task.CompletedTask;
        }
    }
}
