namespace Shopping_Pet.DTOs.Reviews
{
    public class ReviewResponseModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }
}
