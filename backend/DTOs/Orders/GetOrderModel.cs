using System;
using System.Collections.Generic;

namespace Shopping_Pet.DTOs.Orders
{
    public class GetOrderModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string UserFullName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string ShippingAddress { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderItemDetailModel> OrderItems { get; set; }
    }

    public class OrderItemDetailModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}