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
        //public async Task<User> DeleteCourierByManagerAsync(
        //    int input,
        //    [Service] BigFoodContext context)
        //{
        //    var role = context.UserRoles.Where(u => u.UserId == input).FirstOrDefault();
        //    if (role != null && role.RoleId == 4)
        //    {
        //        context.UserRoles.Remove(role);
        //        await context.SaveChangesAsync();
        //    }
        //    var user = context.Users.Where(o => o.Id == input).FirstOrDefault();
        //    if (user != null)
        //    {
        //        context.Users.Remove(user);
        //        await context.SaveChangesAsync();
        //    }
        //    return await Task.FromResult(user);
        //}

        //[Authorize(Roles = new[] { "MANAGER" })]
        //public async Task<UserData> UpdateCourierByManagerAsync(
        //    RegisterUser input,
        //    [Service] BigFoodContext context)
        //{
        //    var role = context.UserRoles.Where(u => u.UserId == input.Id).FirstOrDefault();
        //    var user = context.Users.Where(o => o.Id == input.Id && role.RoleId == 4).FirstOrDefault();
        //    if (user != null)
        //    {
        //        user.Username = input.UserName;
        //        user.Email = input.Email;
        //        user.Password = BCrypt.Net.BCrypt.HashPassword(input.Password);
        //        var ret = context.Users.Update(user);
        //        await context.SaveChangesAsync();
        //    }
        //    return await Task.FromResult(new UserData
        //    {
        //        Id = user.Id,
        //        Username = user.Username,
        //        Email = user.Email,
        //    });
        //}

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
            //var userName = claimsPrincipal.Identity.Name;
            //var courier = context.Users.Where(c => c.Username == userName).FirstOrDefault();
            //var order = context.Orders.Where(o => o.Completed == false && o.CourierLocId == courier.Id).FirstOrDefault();
            //var orderDetail = context.OrderDetails.Where(o => o.OrderId == order.Id).FirstOrDefault();
            //var status = context.CourierLocs.Where(s => s.UserId == courier.Id).FirstOrDefault();

            //using var transaction = context.Database.BeginTransaction();
            //if (order != null)
            //{
            //    order.Completed = true;
            //    context.Orders.Update(order);

            //    status.Status = "AVAILABLE";
            //    context.CourierLocs.Update(status);

            //    await context.SaveChangesAsync();
            //    return await Task.FromResult(order);
            //}
            ///*if (order.Completed != false)
            //{
            //    try
            //    {
            //        if (order != null)
            //        {
            //            order.Completed = true;
            //            context.Orders.Update(order);

            //            status.Status = "AVAILABLE";
            //            context.CourierLocs.Update(status);

            //            await context.SaveChangesAsync();
            //            return await Task.FromResult(order);
            //        }
            //    }
            //    catch
            //    {
            //        transaction.Rollback();
            //    }
            //}*/
            //return await Task.FromResult(order);
        }

        /*public async Task<CourierLoc> UpdateOrderByCourier(
            CourierLoc input,
            [Service] FoodAppContext context)
        {
            var courier = context.CourierLocs.Where(o => o.Id == input.Id).FirstOrDefault();
            if (courier != null)
            {
                courier.GeoLat = Convert.ToString(input.GeoLat);
                courier.GeoLong = Convert.ToString(input.GeoLong);


                context.CourierLocs.Update(courier);
                await context.SaveChangesAsync();
            }
        }*/

      

        /*[Authorize(Roles = new[] { "COURIER" })]
        public async Task<CourierLoc> UpdateCourierLocByBuyer(
            CourierLocInput input,
            [Service] FoodAppContext context)
        {
            CourierLoc courierLoc = new CourierLoc();
        }*/

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
