using FoodDb.Models;
using HotChocolate.AspNetCore.Authorization;

namespace OrderService.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "MANAGER", "BUYER" })] // dapat diakses kalau sudah login
        public IQueryable<Order> GetOrders([Service] FoodAppContext context) =>
            context.Orders;

        [Authorize(Roles = new[] { "MANAGER", "BUYER" })] // dapat diakses kalau sudah login
        public IQueryable<Courier> GetCouriers([Service] FoodAppContext context) =>
            context.Couriers;
    }
}
