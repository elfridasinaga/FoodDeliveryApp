namespace OrderService.GraphQL
{
    public partial class OrderOutput
    {
        public int? Id { get; set; }
        public string? Code { get; set; } = null!;
        public int UserId { get; set; }
        public int FoodId { get; set; }
        public int CourierId { get; set; }
        public int Quantity { get; set; }
    }
}
