using MassTransit;
using Microsoft.AspNetCore.Mvc;
using practice.one.api.Models;
using practice.one.component.Abstractions;
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

        public OrdersController(
            IRequestClient<ISubmitOrder> submitOrderClient, 
            IPublishEndpoint publishEndpoint)
        {
            _submitOrderClient = submitOrderClient;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("Notification")]
        public async Task<IActionResult> Notification([FromBody] MessageNotify notify)
        {
            try
            {
                await _publishEndpoint.Publish<IMessageNotify>(notify);

                return Ok("Message Send");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrder request)
        {
            var (accepted, rejected) = await _submitOrderClient.GetResponse<IOrderSubmitted, IOrderRejected>(request);

            if (accepted.IsCompleted)
            {
                var result = await accepted;

                return Ok("Order Submitted successfuly");
            }

            var response = await rejected;

            return BadRequest(response.Message);

        }
    }
}
