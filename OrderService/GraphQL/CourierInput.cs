namespace OrderService.GraphQL
{
    public record CourierInput
    (
        int? Id,
        string CourierName,
        string PhoneNumber
    );
}
