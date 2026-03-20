using System;
using System.Collections.Generic;

namespace Shopping_Pet.DTOs.Carts
{
    public class GetCartModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; } // Tổng tiền giỏ hàng
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CartItemDetailModel> CartItems { get; set; }
    }

    public class CartItemDetailModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public string? ProductImage { get; set; }
    }
}