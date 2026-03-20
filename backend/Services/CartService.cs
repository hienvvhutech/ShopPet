using Shopping_Pet.DTOs.Carts;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<(bool Success, string Message, int CartItemId)> AddCartItemAsync(string userId, AddCartItemModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null)
                return (false, "Sản phẩm không tồn tại", 0);

            if (product.StockQuantity < model.Quantity)
                return (false, "Không đủ hàng trong kho", 0);

            if (model.UnitPrice != product.Price)
                return (false, "Giá sản phẩm không khớp", 0);

            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>(),

                     SessionId = userId
                };
                await _cartRepository.AddAsync(cart);
            }

            var existingItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id, model.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;
                existingItem.UnitPrice = model.UnitPrice;
                await _cartItemRepository.UpdateAsync(existingItem);
                return (true, "Cập nhật số lượng sản phẩm trong giỏ", existingItem.Id);
            }

            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                UserId = userId
            };
            await _cartItemRepository.AddAsync(newItem);
            return (true, "Thêm sản phẩm vào giỏ thành công", newItem.Id);
        }

        public async Task<(bool Success, string Message)> UpdateCartItemAsync(string userId, int cartItemId, UpdateCartItemModel model)
        {
            var item = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (item == null || item.UserId != userId)
                return (false, "Sản phẩm không tồn tại trong giỏ");

            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                return (false, "Sản phẩm không tồn tại");

            if (product.StockQuantity < model.Quantity)
                return (false, "Không đủ hàng trong kho");

            item.Quantity = model.Quantity;
            item.UnitPrice = product.Price;
            await _cartItemRepository.UpdateAsync(item);
            return (true, "Cập nhật giỏ hàng thành công");
        }

        public async Task<(bool Success, string Message)> RemoveCartItemAsync(string userId, int cartItemId)
        {
            var item = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (item == null || item.UserId != userId)
                return (false, "Sản phẩm không tồn tại trong giỏ");

            await _cartItemRepository.RemoveAsync(item);
            return (true, "Xoá sản phẩm khỏi giỏ hàng thành công");
        }

        public async Task<List<CartItem>> GetCartItemsAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return new List<CartItem>();
            return await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
        }

        public async Task<(bool Success, string Message)> ClearCartAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return (false, "Giỏ hàng không tồn tại");

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            await _cartItemRepository.RemoveRangeAsync(items);
            return (true, "Đã xoá toàn bộ giỏ hàng");
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            return await _cartRepository.GetByUserIdAsync(userId);
        }
        public async Task<GetCartModel?> GetCartDtoAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
                return null;

            var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id);
            var itemDtos = new List<CartItemDetailModel>();

            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                itemDtos.Add(new CartItemDetailModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = product?.Name ?? "Không rõ",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ProductImage = product?.ImageUrl
                });
            }

            return new GetCartModel
            {
                Id = cart.Id,
                UserId = cart.UserId,
                TotalAmount = itemDtos.Sum(i => i.UnitPrice * i.Quantity),
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                CartItems = itemDtos
            };
        }

    }
}
