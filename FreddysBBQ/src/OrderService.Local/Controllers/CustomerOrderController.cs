using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderService.Providers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [Authorize(Policy = "Orders")]
    [Route("/customer")]
    public class CustomerOrderController : Controller
    {
        private readonly IOrderStorage orderStorage;
        private readonly ILogger<CustomerOrderController> logger;
        private readonly IMenuService menuService;

        public CustomerOrderController(IOrderStorage orderStorage, IMenuService menuService, ILogger<CustomerOrderController> logger)
        {
            this.orderStorage = orderStorage;
            this.menuService = menuService;
            this.logger = logger;
        }

        [HttpGet("order")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Order>> GetMyOrders()
        {
            var customerId = GetCustomerId(HttpContext.User.Identity);
            
            if (string.IsNullOrEmpty(customerId))
            {
                logger.LogWarning($"Customer ID {customerId} is not found");

                return Ok(Enumerable.Empty<Order>());
            }
            else
            {
                logger.LogInformation($"Retrieving order for CustomerId ID {customerId}");
            }

            var orders = orderStorage.GetOrderByCustomerId(customerId);

            if(!orders.Any())
            {
                logger.LogWarning($"No orders found for Customer ID {customerId}");
            }

            return Ok(orders);
        }

        [HttpPost("order")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        public async ValueTask<ActionResult<Order>> Post([FromBody]Dictionary<long, int?> itemsAndQuantities)
        {
            if (itemsAndQuantities is null)
            {
                logger.LogCritical("No order items detected!");

                return BadRequest("Empty orders not allowed");
            }

            LogClaims(HttpContext.User.Identity);

            var order = new Order
            {
                CustomerId = GetCustomerId(HttpContext.User.Identity),
                Email = GetEmail(HttpContext.User.Identity),
                FirstName = GetFirstName(HttpContext.User.Identity),
                LastName = GetLastName(HttpContext.User.Identity),
                Total = 0
            };

            if (string.IsNullOrEmpty(order.CustomerId))
            {
                logger.LogCritical("No Customer ID found for authenticated user");

                return BadRequest();
            }
            else
            {
                logger.LogInformation($"Adding order for Customer ID {order.CustomerId}");
            }

            foreach (var reqItem in itemsAndQuantities)
            {
                var itemId = reqItem.Key;
                var quantity = reqItem.Value ?? 0;

                logger.LogDebug($"Order item key: {itemId}, Quantity: {quantity}");

                if (itemId < 0 || quantity < 0)
                {
                    return BadRequest();
                }

                if (quantity == 0)
                {
                    continue;
                }

                MenuItem item = await menuService.GetMenuItemAsync(itemId);

                if (item == null)
                {
                    logger.LogCritical("Unable to find menu item: " + itemId);
                    
                    continue;
                }

                var orderItem = new OrderItem
                {
                    Order = order,
                    Quantity = quantity,
                    MenuItemId = itemId,
                    Name = item.Name,
                    Price = item.Price
                };

                order.OrderItems.Add(orderItem);
                order.Total += (item.Price * quantity);
            }

            if (order.OrderItems.Count > 0)
            {
                var createdOrder = await orderStorage.AddOrderAsync(order);

                return Ok(createdOrder);
            }
            else
            {
                logger.LogCritical("Somehow ended up with no order items");

                return Conflict(
                    @"State of information provided is incorrect. 
                    Please ensure at least one menu item has quantity greater than 0. 
                    Please ensure menu item IDs are still current. ");
            }
        }

        private void LogClaims(IIdentity identity)
        {
            if (identity is not ClaimsIdentity claims)
            {
                logger.LogError("Unable to access ClaimsIdentity");

                return;
            }

            foreach (Claim c in claims.Claims)
            {
                logger.LogInformation(string.Format("Claim: {0}/{1}/{2}", c.Type, c.Value, c.ValueType));
            }
        }

        private string GetLastName(IIdentity identity)
        {
            return string.Empty;
        }

        private string GetFirstName(IIdentity identity)
        {
            return GetClaim(identity, "user_name");
        }

        private string GetEmail(IIdentity identity)
        {
            return GetClaim(identity, ClaimTypes.Email);
        }

        private string GetCustomerId(IIdentity identity)
        {
            return GetClaim(identity, "user_id");
        }

        private string GetClaim(IIdentity identity, string claim)
        {
            if (identity is not ClaimsIdentity claims)
            {
                logger.LogError("Unable to access ClaimsIdentity");

                return null;
            }

            var idClaim = claims.FindFirst(claim);

            if (idClaim is null)
            {
                logger.LogError($"Unable to access: {claim}");

                return null;
            }

            return idClaim.Value;
        }
    }
}
