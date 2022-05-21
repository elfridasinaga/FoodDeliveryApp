using FoodDb.Models;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Mutation
    {

        [Authorize(Roles = new[] { "BUYER" })]

        public async Task<OrderOutput> AddOrderByBuyerAsync(
            OrderOutput input,
            ClaimsPrincipal claimsPrincipal,
            [Service] FoodAppContext context)
        {
            var userName = claimsPrincipal.Identity.Name;
            var user = context.Users.Where(u => u.Username == userName).FirstOrDefault();
            var courierloc = context.CourierLocs.Where(c => c.Id == input.CourierId).FirstOrDefault();

            using var transaction = context.Database.BeginTransaction();
            try
            {
                if (courierloc.Status == "AVAILABLE")
                {

                    var order = new Order
                    {
                        Code = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        CourierLocId = input.CourierId,
                        Completed = false
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        FoodId = input.FoodId,
                        Quantity = input.Quantity
                    };
                    order.OrderDetails.Add(detail);
                    context.SaveChanges();

                    courierloc.Status = "UNAVAILABLE";
                    context.CourierLocs.Update(courierloc);
                    context.SaveChanges();

                    await transaction.CommitAsync();
                }
                else
                    throw new Exception("user was not found");
            }
            catch (Exception err)
            {
                transaction.Rollback();
            }

            return input;
        }

        [Authorize(Roles = new[] { "COURIER" })]
        public async Task<UpdateCourier> UpdateOrderByCourier(
            UpdateCourier input,
            [Service] FoodAppContext context)
        {
            var courierloc = context.CourierLocs.Where(c => c.Id == input.OrderId).FirstOrDefault();
            if(courierloc != null)
            {
                courierloc.GeoLat = Convert.ToString(input.GeoLat);
                courierloc.GeoLong = Convert.ToString(input.GeoLong);

                context.CourierLocs.Update(courierloc);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(input);
        }

        [Authorize(Roles = new[] { "COURIER" })]
        public async Task<Order> CompletedOrderByCourier(
            int id,
            [Service] FoodAppContext context)
        {
            var order = context.Orders.Where(o => o.Id == id).FirstOrDefault();
            var status = context.CourierLocs.Where(c => c.Id == order.CourierLocId).FirstOrDefault();
            if (order != null)
            {
                order.Completed = true;
                context.Orders.Update(order);

                status.Status = "AVAILABLE";
                context.CourierLocs.Update(status);
                context.SaveChanges();
            }
            return await Task.FromResult(order);
            
        }

    }
}
