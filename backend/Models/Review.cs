namespace Shopping_Pet.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsHidden { get; set; } 

        public ApplicationUser User { get; set; }
        public Product Product { get; set; }
    }
}
