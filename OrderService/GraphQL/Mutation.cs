using FoodDb.Models;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Mutation
    {
        //[Authorize(Roles = new[] { "MANAGER" })]

        public async Task<OrderData> AddOrderAsync(
            OrderData input,
            ClaimsPrincipal claimsPrincipal,
            [Service] FoodAppContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            var userName = claimsPrincipal.Identity.Name;

            try
            {
                var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
                if (user != null)
                {
                    // EF
                    var order = new Order
                    {
                        Code = Guid.NewGuid().ToString(), // generate random chars using GUID
                        UserId = user.Id,
                        CourierId = input.CourierId
                    };
                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var item in input.Details)
                    {
                        var detail = new OrderDetail
                        {
                            OrderId = order.Id,
                            FoodId = item.FoodId,
                            Quantity = item.Quantity
                        };
                        order.OrderDetails.Add(detail);
                    }
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

        //[Authorize(Roles = new[] { "MANAGER" })]
        public async Task<Order> DeleteOrderByIdAsync(
        int id,
        [Service] FoodAppContext context)
        {
            var order = context.Orders.Where(u => u.Id == id).Include(u => u.OrderDetails).FirstOrDefault();
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
            return await Task.FromResult(order);
        }

        //Courier
        //[Authorize(Roles = new[] { "MANAGER" })]
        //public async Task<Courier> AddCourierAsync(
        //    CourierInput input,
        //    [Service] FoodAppContext context)
        //{

        //    // EF
        //    var courier = new Courier
        //    {
        //        CourierName = input.CourierName,
        //        PhoneNumber = input.PhoneNumber
        //    };

        //    var ret = context.Couriers.Add(courier);
        //    await context.SaveChangesAsync();

        //    return ret.Entity;
        //}

        //[Authorize(Roles = new[] { "MANAGER" })]
        //public async Task<Courier> UpdateCourierAsync(
        //    CourierInput input,
        //    [Service] FoodAppContext context)
        //{
        //    var courier = context.Couriers.Where(o => o.Id == input.Id).FirstOrDefault();
        //    if (courier != null)
        //    {
        //        courier.CourierName = input.CourierName;


        //        context.Couriers.Update(courier);
        //        await context.SaveChangesAsync();
        //    }


        //    return await Task.FromResult(courier);
        //}

        //[Authorize(Roles = new[] { "MANAGER" })]
        //public async Task<Courier> DeleteCourierByIdAsync(
        //int id,
        //[Service] FoodAppContext context)
        //{
        //    var courier = context.Couriers.Where(u => u.Id == id).FirstOrDefault();
        //    if (courier != null)
        //    {
        //        context.Couriers.Remove(courier);
        //        await context.SaveChangesAsync();
        //    }
        //    return await Task.FromResult(courier);
        //}
    }
}
