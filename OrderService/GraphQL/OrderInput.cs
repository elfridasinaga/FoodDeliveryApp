namespace OrderService.GraphQL
{
    public record OrderInput
    (
        int? Id,
        string? Code,
        int UserId,
        int FoodId,
        int CourierId,
        int Quantity
        );
}
