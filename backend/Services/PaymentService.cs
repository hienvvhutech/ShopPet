using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shopping_Pet.Data;
using Shopping_Pet.DTOs.Payments;
using Shopping_Pet.Models;

namespace Shopping_Pet.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int PaymentId)> CreatePaymentAsync(string userId, CreatePaymentModel model)
        {
            var validMethods = new[] { "MoMo", "VNPay" };
            if (!validMethods.Contains(model.PaymentMethod))
            {
                return (false, "Phương thức thanh toán không hợp lệ", 0);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == model.OrderId && o.UserId == userId);

                if (order == null)
                {
                    return (false, "Đơn hàng không tồn tại hoặc không có quyền", 0);
                }

                if (model.Amount != order.TotalAmount)
                {
                    return (false, "Số tiền thanh toán không khớp với đơn hàng", 0);
                }

                var payment = new Payment
                {
                    OrderId = model.OrderId,
                    Amount = model.Amount,
                    PaymentMethod = model.PaymentMethod,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Tạo thanh toán thành công", payment.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Lỗi khi tạo thanh toán", 0);
            }
        }

        public async Task<Payment> GetPaymentByOrderAsync(int orderId, string userId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId && p.Order.UserId == userId);
        }

        public async Task<(bool Success, string Message)> UpdatePaymentStatusAsync(int paymentId, string status, string transactionId = null)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                return (false, "Thanh toán không tồn tại");
            }

            payment.Status = status;
            payment.TransactionId = transactionId ?? payment.TransactionId;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Cập nhật trạng thái thanh toán thành công");
        }
    }
}