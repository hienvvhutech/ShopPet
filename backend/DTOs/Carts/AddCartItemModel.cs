using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Carts
{
    public class AddCartItemModel
    {
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal UnitPrice { get; set; }
    }
}