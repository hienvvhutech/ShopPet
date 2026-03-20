namespace Shopping_Pet.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string? SenderId { get; set; } = null!;
        public string ReceiverId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}
