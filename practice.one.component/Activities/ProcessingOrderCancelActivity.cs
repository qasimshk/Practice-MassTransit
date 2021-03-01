using MassTransit.Courier;
using System;
using System.Threading.Tasks;

namespace practice.one.component.Activities
{
    public class ProcessingOrderCancelActivity : IExecuteActivity<OrderCancelledArguements>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<OrderCancelledArguements> context)
        {
            Guid orderId = context.Arguments.OrderId;
            string reason = context.Arguments.Reason;

            if (reason == "test")
                throw new InvalidOperationException("reason cannot be test");

            await Task.Delay(1000);

            return context.Completed();
        }
    }

    public interface OrderCancelledArguements
    {
        Guid OrderId { get; }
        string Reason { get; }
    }
}
