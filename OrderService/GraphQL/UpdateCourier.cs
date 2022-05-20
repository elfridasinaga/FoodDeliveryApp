namespace OrderService.GraphQL
{
    public record UpdateCourier
    (
        double GeoLat,
        double GeoLong,
        int OrderId
        );
}
