using System;
using System.Collections.Generic;

namespace FoodDb.Models
{
    public partial class CourierLoc
    {
        public CourierLoc()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string? Status { get; set; }
        public string? GeoLat { get; set; }
        public string? GeoLong { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
