using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.Discovery;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Services
{
    public class OrderService : AbstractService, IOrderService
    {
        private readonly string ordersUrl;

        public OrderService(
            IOptions<ServiceUrl> urlOptions,
            IDiscoveryClient client,
            ILogger<OrderService> logger,
            IHttpContextAccessor context) : base(client, logger, context)
        {
            ordersUrl = urlOptions.Value.OrderServiceUrl;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var client = await GetClientAsync();
            var request = GetRequest(HttpMethod.Get, ordersUrl);
            return await DoRequest<List<Order>>(client, request);
        }

        public async Task RemoveOrderAsync(long id)
        {
            var client = await GetClientAsync();
            var url = ordersUrl + "/" + id.ToString();
            var request = GetRequest(HttpMethod.Post, url);
            await DoRequest(client, request);
        }
    }
}
