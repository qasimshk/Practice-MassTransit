using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using practice.one.api.Models;
using practice.one.component.Abstractions;
using practice.one.component.Activities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace practice.one.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IRequestClient<OrderFry> _orderFryClient;
        private readonly IRequestClient<ISubmitOrder> _submitOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBus _bus;
        readonly Uri _processingOrderCancel;

        public OrdersController(IRequestClient<OrderFry> OrderFryClient, IRequestClient<ISubmitOrder> submitOrderClient, IPublishEndpoint publishEndpoint, IBus bus, IEndpointNameFormatter formatter)
        {
            _orderFryClient = OrderFryClient;
            _submitOrderClient = submitOrderClient;
            _publishEndpoint = publishEndpoint;
            _bus = bus;

            _processingOrderCancel = new Uri($"queue:{formatter.ExecuteActivity<ProcessingOrderCancelActivity, OrderCancelledArguements>()}");
        }

        [HttpPost("Notification")]
        public async Task<IActionResult> Notification([FromBody] MessageNotify notify)
        {
            try
            {
                await _publishEndpoint.Publish<IMessageNotify>(notify);

                //wait _bus.Publish(notify);

                return Ok("Message Send");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("OrderCancel")]
        public async Task<IActionResult> OrderCancel([FromBody] OrderCancel cancel)
        {
            try
            {
                var builder = new RoutingSlipBuilder(NewId.NextGuid());
                //var result = builder.AddSubscription(new Uri("rabbitmqs://rattlesnake.rmq.cloudamqp.com/fipkceeo/OrderCancelled_execute"), RoutingSlipEvents.All,
                //    async x =>
                //    {
                //        await x.Send(cancel);
                //    });


                builder.AddActivity("OrderCancelled", new Uri("queue:OrderCancelled_execute"), new
                {
                    Reason = cancel.Reason
                });

                builder.AddVariable("OrderId", cancel.OrderId);



                builder.AddActivity(nameof(ProcessingOrderCancelActivity), new Uri("queue:ProcessingOrderCancel_execute"), new
                {
                    Reason = cancel.Reason
                });

                builder.AddVariable("OrderId", cancel.OrderId);


                var routingSlip = builder.Build();
                await _bus.Execute(routingSlip);

                //Publisher Consumer 
                //await _publishEndpoint.Publish<IPaymentReturn>(new PaymentReturn(NewId.NextGuid(), 0));

                return Ok(builder.Build());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SagaOrder")]
        public async Task<IActionResult> SagaOrder([FromBody] SagaeOrderRequest request)
        {
            Response response = await _orderFryClient.GetResponse<OrderFryCompleted, OrderFaulted>(request);

            return response switch
            {
                (_, OrderFryCompleted completed) => Ok(new
                {
                    completed.OrderId,
                    completed.Created,
                    completed.Completed, 

                    completed.OrderRefrence,
                    completed.ProductName,
                    completed.Quantity
                }),
                (_, OrderFaulted faulted) => BadRequest(new
                {
                    faulted.OrderId,
                    faulted.Created,
                    faulted.Faulted,
                    LinesCompleted = faulted.LinesCompleted.ToDictionary(x => x.Key, x => new
                    {
                        x.Value.Created,
                        x.Value.Completed,
                        x.Value.Description,
                    }),
                    LinesFaulted = faulted.LinesFaulted.ToDictionary(x => x.Key, x => new
                    {
                        Faulted = x.Value.Timestamp,
                        Reason = x.Value.GetExceptionMessages(),
                    })
                }),
                _ => BadRequest()
            };
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrder request)
        {
            if (request == null)
                return BadRequest("empty Request");

            var (accepted, rejected) = await _submitOrderClient.GetResponse<IOrderSubmitted, IOrderRejected>(request);

            if (accepted.IsCompletedSuccessfully)
            {
                var result = await accepted;

                return Accepted("Order Submitted successfuly");
            }

            //if (accepted.IsCompleted)
            //{
            //    var result = await accepted;

            //    return Ok("Order Submitted successfuly");
            //}
            else
            {
                var response = await rejected;

                return BadRequest(response.Message);
            }
        }
    }

    public class PaymentReturn
    {
        public Guid OrderId { get; }
        public int Amouunt { get; }

        public PaymentReturn(Guid orderId, int amouunt)
        {
            OrderId = orderId;
            Amouunt = amouunt;
        }
    }

    public class SagaeOrderRequest : OrderFry
    {
        public Guid OrderRefrence { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public Guid OrderId { get; set; }

        public Guid OrderLineId { get; set; }
    }

    public static class FutureContractExtensions
    {
        public static string GetExceptionMessages(this Fault faulted)
        {
            return faulted.Exceptions != null ? string.Join(Environment.NewLine, faulted.Exceptions.Select(x => x.Message)) : string.Empty;
        }
    }
}
