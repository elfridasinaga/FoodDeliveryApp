using FoodDb.Models;
using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;

namespace FoodDelivery.GraphQL
{
    public class Query
    {
        /*[Authorize(Roles = new[] { "MANAGER" })] // dapat diakses kalau sudah login
        public IQueryable<Food> GetFoods([Service] FoodAppContext context) =>
            context.Foods;*/

        [Authorize]
        public IQueryable<Food> GetFoods([Service] FoodAppContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;

            // check manager role ?
            var managerRole = claimsPrincipal.Claims.Where(o => o.Type == ClaimTypes.Role).FirstOrDefault();
            var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (user != null)
            {
                if (managerRole.Value == "MANAGER" || managerRole.Value == "BUYER")
                {
                    return context.Foods;
                }
                var foods = context.Foods;
                return foods.AsQueryable();
            }
            return new List<Food>().AsQueryable();
        }

        /*[Authorize]
        public IQueryable<Food> GetFoodsbyToken([Service] FoodAppContext context, ClaimsPrincipal claimsPrincipal)
        {
            var userName = claimsPrincipal.Identity.Name;
            var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
            if (user != null)
            {
                var food = context.Foods.Where(o => o.Id == user.Id);
                return food.AsQueryable();
            }
            return new List<Food>().AsQueryable();
        }*/
    }
}
