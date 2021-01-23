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
    public class MenuService : AbstractService, IMenuService
    {
        private readonly string menuItemsUrl;

        public MenuService(
            IOptions<ServiceUrl> urlOptions,
            IDiscoveryClient client, 
            ILogger<MenuService> logger,
            IHttpContextAccessor context) :
            base(client, logger, context)
        {
            menuItemsUrl = urlOptions.Value.MenuServiceUrl;
        }

        public async Task<MenuItem> GetMenuItemAsync(long id)
        {
            var client = await GetClientAsync();
            var requestUri = menuItemsUrl.Replace("{id}", id.ToString());
            var request = GetRequest(HttpMethod.Get, requestUri);

            return await DoRequest<MenuItem>(client, request);
        }

        public async Task<List<MenuItem>> GetMenuItemsAsync()
        {
            var client = await GetClientAsync();
            var requestUri = menuItemsUrl.Replace("/{id}", string.Empty);
            var request = GetRequest(HttpMethod.Get, requestUri);

            return await DoHateoasRequest<List<MenuItem>>(client, request, "menuItems");
        }
        
        public async Task SaveMenuItemAsync(MenuItem item, bool newOne = false)
        {
            var client = await GetClientAsync();

            var requestUri = menuItemsUrl.Replace("{id}", item.Id.ToString());
            HttpMethod method = HttpMethod.Put;

            if (newOne)
            {
                method = HttpMethod.Post;
                requestUri = menuItemsUrl.Replace("/{id}", string.Empty);
            } 

            var request = GetRequest(method, requestUri);
            request.Content = GetRequestContent(item);

            await DoRequest(client, request);
        }

        public async Task DeleteMenuItemAsync(long id)
        {
            var client = await GetClientAsync();
            var requestUri = menuItemsUrl.Replace("{id}", id.ToString());
            var request = GetRequest(HttpMethod.Delete, requestUri);

            await DoRequest(client, request);
        }
    }
}
