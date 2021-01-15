using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Providers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderStorage orderStorage;
        private readonly ILogger<OrderController> logger;

        public OrderController(IOrderStorage orderStorage, ILogger<OrderController> logger)
        {
            this.orderStorage = orderStorage;
            this.logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public IEnumerable<Order> Get()
        {
            logger.LogInformation("Retrieving orders");

            return orderStorage.GetOrders();
        }

        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        public async ValueTask<ActionResult<Order>> Delete(int id)
        {
            logger.LogInformation($"Deleting order #{id}");

            var order = orderStorage.GetOrderById(id);

            if(order is null)
            {
                return NotFound($"Order #{id} does not exist in the system");
            }

            var removedOrder = await orderStorage.DeleteOrderAsync(order);

            return Ok(removedOrder);
        }
    }
}
