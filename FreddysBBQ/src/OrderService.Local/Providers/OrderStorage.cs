using Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
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

        public Order GetOrderById(int id)
        {
            return Orders
                .Where(order => order.Id == id)
                .Include(order => order.OrderItems)
                .FirstOrDefault();
        }

        public async ValueTask<Order> DeleteOrderAsync(Order order)
        {
            foreach(var item in order.OrderItems)
            {
                DeleteOrderItem(item);
            }

            var deletedOrder = Orders.Remove(order).Entity;

            await SaveChangesAsync();

            return deletedOrder;
        }

        private OrderItem DeleteOrderItem(OrderItem orderItem) =>
            OrderItems.Remove(orderItem).Entity;
    }
}
