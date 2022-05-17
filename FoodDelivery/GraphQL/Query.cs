using FoodDelivery.Models;
using HotChocolate.AspNetCore.Authorization;

namespace FoodDelivery.GraphQL
{
    public class Query
    {
        [Authorize(Roles = new[] { "BUYER" })] // dapat diakses kalau sudah login
        public IQueryable<Food> GetFoods([Service] FoodAppContext context) =>
            context.Foods;
    }
}
