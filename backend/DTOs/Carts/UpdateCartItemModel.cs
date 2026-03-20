using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Carts
{
    public class UpdateCartItemModel
    {
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } // Số lượng
    }
}
