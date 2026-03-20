namespace Shopping_Pet.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // MoMo, VNPay
        public string Status { get; set; } // Pending, Success, Failed
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // Thêm để theo dõi cập nhật

        public Order Order { get; set; }
    }
}
