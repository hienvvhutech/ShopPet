namespace Shopping_Pet.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? OrderId { get; set; }
    }
}
