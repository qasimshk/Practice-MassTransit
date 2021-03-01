using MassTransit.Courier;
using practice.one.component.Activities;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace practice.one.service.Activities
{
    public class OrderCancelledActivity : IActivity<OrderCancelledArguements, OrderCancelledLog>
    {
        private readonly ILogger<OrderCancelledActivity> _logger;

        public OrderCancelledActivity(ILogger<OrderCancelledActivity> logger)
        {
            _logger = logger;
        }

        public async Task<CompensationResult> Compensate(CompensateContext<OrderCancelledLog> context)
        {
            _logger.LogInformation("since it is test order cannot be cancelled");

            return context.Compensated();
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<OrderCancelledArguements> context)
        {
            //return context.Terminate(new { Reason = "Not a good time, dude." });

            return context.CompletedWithVariables<OrderCancelledLog>(
                new
                {
                    OrderId = context.Arguments.OrderId,
                    Reason = context.Arguments.Reason
                },
                new {
                OrderId = context.Arguments.OrderId,
                Reason = context.Arguments.Reason
            });

            //return context.CompletedWithVariables<OrderCancelledLog>(new { Reason = "Qasim" }, new { Reason = "Qasim" });
        }
    }
}
