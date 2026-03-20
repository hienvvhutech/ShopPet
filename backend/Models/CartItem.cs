namespace Shopping_Pet.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public string UserId { get; set; } = null!;

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
     
    }
}
