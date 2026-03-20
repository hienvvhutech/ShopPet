using Shopping_Pet.DTOs.Reviews;
using Shopping_Pet.Models;
using Shopping_Pet.Repositories.Interfaces;

namespace Shopping_Pet.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            IOrderItemRepository orderItemRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<(bool Success, string Message, int ReviewId)> CreateReviewAsync(string userId, CreateReviewModel model)
        {
            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null)
                return (false, "Sản phẩm không tồn tại", 0);

            var hasPurchased = await _orderItemRepository.HasUserPurchasedProductAsync(userId, model.ProductId);
            if (!hasPurchased)
                return (false, "Bạn phải mua sản phẩm để đánh giá", 0);

            var review = new Review
            {
                UserId = userId,
                ProductId = model.ProductId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow,
                IsHidden = false
            };

            await _reviewRepository.AddAsync(review);
            return (true, "Tạo đánh giá thành công", review.Id);
        }

        public async Task<List<Review>> GetReviewsByProductAsync(int productId)
        {
            return await _reviewRepository.GetVisibleByProductIdAsync(productId);
        }

        public async Task<(bool Success, string Message)> UpdateReviewAsync(int reviewId, string userId, CreateReviewModel model, string userRole)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return (false, "Đánh giá không tồn tại");

            if (userRole != "Admin" && review.UserId != userId)
                return (false, "Không có quyền cập nhật");

            review.Rating = model.Rating;
            review.Comment = model.Comment;

            await _reviewRepository.UpdateAsync(review);
            return (true, "Cập nhật đánh giá thành công");
        }

        public async Task<(bool Success, string Message)> DeleteReviewAsync(int reviewId, string userId, string userRole)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return (false, "Đánh giá không tồn tại");

            if (userRole != "Admin" && review.UserId != userId)
                return (false, "Không có quyền xóa");

            await _reviewRepository.DeleteAsync(review);
            return (true, "Xóa đánh giá thành công");
        }

        public async Task<(bool Success, string Message)> HideReviewAsync(int reviewId, bool isHidden)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return (false, "Đánh giá không tồn tại");

            review.IsHidden = isHidden;
            await _reviewRepository.UpdateAsync(review);
            return (true, isHidden ? "Đánh giá đã bị ẩn" : "Đánh giá đã được hiện");
        }
        public async Task<List<ReviewResponseModel>> GetReviewsByUserAsync(string userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);

            return reviews.Select(r => new ReviewResponseModel
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product?.Name ?? "Sản phẩm đã bị xoá",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }
        public async Task<List<ReviewResponseModel>> GetAllAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return reviews.Select(r => new ReviewResponseModel
            {
                Id = r.Id,
                ProductId = r.ProductId,
                ProductName = r.Product?.Name ?? "Sản phẩm đã bị xoá",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UserName = r.User?.FullName ?? "Tài khoản đã xoá",
                IsHidden = r.IsHidden
            }).ToList();
        }

    }
}
