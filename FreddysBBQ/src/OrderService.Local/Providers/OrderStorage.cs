using Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Providers
{
    public class OrderStorage : DbContext, IOrderStorage
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderStorage(DbContextOptions<OrderStorage> options)
            : base(options) { }

        public IEnumerable<Order> GetOrders() =>
            Orders.Include(order => order.OrderItems);

        public Order GetOrderById(int id) =>
            Orders
                .Where(order => order.Id == id)
                .Include(order => order.OrderItems)
                .FirstOrDefault();

        public async ValueTask<Order> DeleteOrderAsync(Order order)
        {
            foreach(var item in order.OrderItems)
            {
                DeleteOrderItem(item);
            }

            var deletedOrderEntityEntry = Orders.Remove(order);

            await SaveChangesAsync();

            return deletedOrderEntityEntry.Entity;
        }

        private OrderItem DeleteOrderItem(OrderItem orderItem) =>
            OrderItems.Remove(orderItem).Entity;

        public IEnumerable<Order> GetOrderByCustomerId(string id) =>
            Orders.Where(o => o.CustomerId == id).Include(o => o.OrderItems);

        public async ValueTask<Order> AddOrderAsync(Order order)
        {
            var orderEntityEntry = Orders.Add(order);

            await SaveChangesAsync();

            return orderEntityEntry.Entity;
        }
    }
}
