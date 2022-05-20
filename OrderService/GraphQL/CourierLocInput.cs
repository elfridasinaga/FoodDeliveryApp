namespace OrderService.GraphQL
{
    public record CourierLocInput
    (
        int? Id,
        string? Status,
        string? GeoLat,
        string? GeoLong,
        int Userid
        );
}
