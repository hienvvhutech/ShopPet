using Shopping_Pet.DTOs.Orders;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly INotificationRepository _notificationRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            INotificationRepository notificationRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<(bool Success, string Message, int OrderId)>
            CreateOrderAsync(string userId, CreateOrderModel model)
        {
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in model.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    return (false, "Sản phẩm không tồn tại", 0);

                if (product.StockQuantity < item.Quantity)
                    return (false, "Không đủ hàng", 0);

                totalAmount += item.Quantity * item.UnitPrice;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                product.StockQuantity -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            var order = new Order
            {
                UserId = userId,
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email,
                ShippingAddress = model.ShippingAddress,
                Note = model.Note,

                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);


            await _notificationRepository.AddAsync(new Notification
            {
                UserId = userId,
                Title = "Đặt hàng thành công",
                Content = $"Đơn hàng #{order.Id} đã được tạo",
                OrderId = order.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            return (true, "Tạo đơn hàng thành công", order.Id);
        }

        public async Task<List<GetOrderModel>> GetOrdersAsync(string userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            return orders.Select(o => new GetOrderModel
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            }).ToList();
        }

        public async Task<GetOrderModel?> GetOrderByIdAsync(int id, string userId)
        {
            var order = await _orderRepository.GetByIdAndUserAsync(id, userId);
            if (order == null) return null;

            return new GetOrderModel
            {
                Id = order.Id,
                UserId = order.UserId,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Email = order.Email,
                ShippingAddress = order.ShippingAddress,
                Note = order.Note,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,

                OrderItems = order.OrderItems.Select(i => new OrderItemDetailModel
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        public async Task<(bool Success, string Message)>
            UpdateOrderStatusAsync(int id, string userId, string status, string role)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                return (false, "Đơn hàng không tồn tại");

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            await _notificationRepository.AddAsync(new Notification
            {
                UserId = order.UserId,
                Title = "Cập nhật đơn hàng",
                Content = $"Đơn hàng #{order.Id} đã chuyển sang trạng thái {status}",
                OrderId = order.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            return (true, "Cập nhật trạng thái thành công");
        }

        public async Task<(bool Success, string Message)>
            CancelOrderAsync(int id, string userId)
        {
            var order = await _orderRepository.GetByIdAndUserAsync(id, userId);
            if (order == null)
                return (false, "Không tìm thấy đơn hàng");

            if (order.Status != "Pending")
                return (false, "Không thể hủy đơn");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);


            await _notificationRepository.AddAsync(new Notification
            {
                UserId = userId,
                Title = "Đơn hàng đã bị huỷ",
                Content = $"Đơn hàng #{order.Id} đã được huỷ",
                OrderId = order.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

            return (true, "Đã hủy đơn hàng");
        }

        public async Task<List<GetOrderModel>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(o => new GetOrderModel
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            }).ToList();
        }
    }
}
