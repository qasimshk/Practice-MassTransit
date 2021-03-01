using MassTransit;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using practice.one.api.Models;
using practice.one.component.Abstractions;
using practice.one.component.Activities;
using System;
using System.Threading.Tasks;

namespace practice.one.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly IRequestClient<ISubmitOrder> _submitOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBus _bus;
        readonly Uri _processingOrderCancel;

        public OrdersController(IRequestClient<ISubmitOrder> submitOrderClient, IPublishEndpoint publishEndpoint, IBus bus, IEndpointNameFormatter formatter)
        {
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


}
