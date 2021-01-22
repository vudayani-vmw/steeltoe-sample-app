using Common.Models;
using Common.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminPortal.Controllers
{
    [Authorize(Policy = "MenuWrite")]
    public class HomeController : Controller
    {
        private readonly Branding branding;
        private readonly IMenuService menuService;
        private readonly IOrderService orderService;
        private readonly ILogger<HomeController> logger;

        public HomeController(
            IOptions<Branding> branding,
            IMenuService menuService,
            IOrderService orderService, 
            ILogger<HomeController> logger)
        {
            this.branding = branding.Value;
            this.logger = logger;
            this.menuService = menuService;
            this.orderService = orderService;
        }

        public IActionResult Index()
        {
            ViewData["username"] = HttpContext.User.Identity.Name;
            ViewData["restaurantName"] = branding.RestaurantName;
            ViewData["Title"] = "Freddy's BBQ";
            return View();
        }

        public async Task<IActionResult> Orders()
        {
            ViewData["Title"] = "Freddy's BBQ Orders";

            var orders = await orderService.GetOrdersAsync();

            return View(orders ?? new List<Order>());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(long id)
        {
            await orderService.RemoveOrderAsync(id);

            return RedirectToAction("Orders");
        }

        public async Task<IActionResult> MenuItems()
        {
            ViewData["Title"] = "Freddy's BBQ Menu Management";
            ViewData["menuTitle"] = branding.MenuTitle;

            var result = await menuService.GetMenuItemsAsync();

            return View(result);
        }

        public async Task<IActionResult> MenuItem(long id)
        {
            ViewData["menuTitle"] = branding.MenuTitle;

            if (id >= 0)
            {
                var result = await menuService.GetMenuItemAsync(id);

                return View(result);
            }
            else
            {
                return View(new MenuItem());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveMenuItem(long id, MenuItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            logger.LogInformation(string.Format("MenuItem: {0}, {1}, {2}", item.Id, item.Name, item.Price));
            
            if (id == -1)
            {
                await menuService.SaveMenuItemAsync(item, true);

                return RedirectToAction("Index");
            }
            else
            {
                await menuService.SaveMenuItemAsync(item, false);

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMenuItem(long id)
        {
            await menuService.DeleteMenuItemAsync(id);

            return RedirectToAction("Index");
        }

        public IActionResult Error() =>
            View();

        [AllowAnonymous]
        public async Task<IActionResult> ViewToken()
        {
            ViewData["token"] = await HttpContext.GetTokenAsync("access_token");

            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            ViewData["Message"] = "Insufficient permissions.";

            return View();
        }
    }
}
