namespace Shopping_Pet.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 
        public string ShippingAddress { get; set;}
        public string Note { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
