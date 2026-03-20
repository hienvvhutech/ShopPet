namespace Shopping_Pet.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!; 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new();


    }
}
