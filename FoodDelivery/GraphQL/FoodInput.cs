﻿namespace FoodDelivery.GraphQL
{
    public record FoodInput
    (
        int Id,
        string Name,
        int Stock,
        double Price
    );
}
