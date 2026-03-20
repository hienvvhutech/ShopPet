using System.ComponentModel.DataAnnotations;

namespace Shopping_Pet.DTOs.Reviews
{
    public class CreateReviewModel
    {
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự")]
        public string Comment { get; set; }
    }
}   