using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Products
{
    public class CreateProductModel
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int CategoryId { get; set; }

    }
}