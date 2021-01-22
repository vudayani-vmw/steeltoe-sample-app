using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Providers
{
    public interface IOrderStorage
    {
        IEnumerable<Order> GetOrders();

        Order GetOrderById(int id);

        ValueTask<Order> DeleteOrderAsync(Order order);

        IEnumerable<Order> GetOrderByCustomerId(string id);

        ValueTask<Order> AddOrderAsync(Order order);
    }
}
