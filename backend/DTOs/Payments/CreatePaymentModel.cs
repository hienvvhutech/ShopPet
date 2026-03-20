using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Payments
{
    public class CreatePaymentModel
    {
        [Required(ErrorMessage = "ID đơn hàng là bắt buộc")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn hoặc bằng 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
        public string PaymentMethod { get; set; }
    }
}