using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Shopping_Pet.DTOs.Orders
{
    public class CreateOrderModel
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
        public string ShippingAddress { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Danh sách sản phẩm là bắt buộc")]
        [MinLength(1, ErrorMessage = "Phải có ít nhất một sản phẩm trong đơn hàng")]
        public List<OrderItemModel> OrderItems { get; set; }
    }

    public class OrderItemModel
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