namespace OrderService.GraphQL
{
    public partial class OrderUpdate
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int UserId { get; set; }
        public int CourierLocId { get; set; }
        public bool Completed { get; set; }

    }
}
