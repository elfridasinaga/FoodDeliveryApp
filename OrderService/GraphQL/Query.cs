using FoodDb.Models;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Query
    {
        public IQueryable<Order> GetOrders([Service] FoodAppContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;
            // check manager role ?
            var managerRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role && o.Value == "MANAGER").FirstOrDefault();
            var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (user != null)
            {
                if (managerRole != null)
                    return context.Orders.Include(o => o.OrderDetails);

                var orders = context.Orders.Where(o => o.UserId == user.Id);
                return orders.AsQueryable();
            }

            return new List<Order>().AsQueryable();
        }

        [Authorize(Roles = new[] { "MANAGER" })]
        public IQueryable<Order> GetOrderByManager([Service] FoodAppContext context) =>
            context.Orders.Include(o => o.OrderDetails);


        [Authorize(Roles = new[] { "BUYER" })]
        public IQueryable<Order> TrackOrderByBuyer([Service] FoodAppContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userToken = claimsPrincipal.Identity;
            var user = context.Users.Where(u => u.Username == userToken.Name).FirstOrDefault();
            if (user != null)
            {
                var order = context.Orders.Where(p => p.UserId == user.Id && p.Completed == false).Include(o => o.OrderDetails);
                return order.AsQueryable();
            }
            return new List<Order>().AsQueryable();
        }

        [Authorize(Roles = new[] { "BUYER" })]
        public IQueryable<Order?> GetOrderByBuyer([Service] FoodAppContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userToken = claimsPrincipal.Identity;
            var user = context.Users.Where(u => u.Username == userToken.Name).FirstOrDefault();
            if (user != null)
            {
                var order = context.Orders.Where(p => p.UserId == user.Id).Include(o => o.OrderDetails);
                return order.AsQueryable();
            }
            return new List<Order>().AsQueryable();
        }

    }
}
