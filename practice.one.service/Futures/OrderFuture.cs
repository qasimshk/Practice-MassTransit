using MassTransit;
using MassTransit.Futures;
using practice.one.component.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace practice.one.service.Futures
{
    public class OrderFuture : Future<OrderFry, OrderFryCompleted, OrderFaulted>
    {
        public OrderFuture()
        {
            ConfigureCommand(x => x.CorrelateById(context => context.Message.OrderRefrence));

            SendRequest<CookFry>(x =>
            {
                x.UsingRequestFactory(context => new CookFryRequest(context.Message.OrderRefrence, context.Message.ProductName, context.Message.Quantity));
            })
                .OnResponseReceived<FryReady>(x =>
                {
                    x.SetCompletedUsingFactory(context => new FryCompletedResult(
                        context.Instance.Created,
                        context.Instance.Completed ?? DateTime.Now,
                        context.Message.OrderId,
                        context.Message.OrderLineId,
                        context.Message.OrderRefrence,
                        context.Message.ProductName, 
                        context.Message.Quantity));
                });

            WhenAllCompleted(r => r.SetCompletedUsingInitializer(context => new
            {
                LinesCompleted = context.Instance.Results.Select(x => x.Value.ToObject<OrderLineCompleted>()).ToDictionary(x => x.OrderLineId),
            }));

            WhenAnyFaulted(f => f.SetFaultedUsingInitializer(context => MapOrderFaulted(context)));
        }

        static object MapOrderFaulted(FutureConsumeContext context)
        {
            Dictionary<Guid, Fault> faults = context.Instance.Faults.ToDictionary(x => x.Key, x => x.Value.ToObject<Fault>());

            return new
            {
                LinesCompleted = context.Instance.Results.ToDictionary(x => x.Key, x => x.Value.ToObject<OrderLineCompleted>()),
                LinesFaulted = faults,
                Exceptions = faults.SelectMany(x => x.Value.Exceptions).ToArray()
            };
        }
    }
}
