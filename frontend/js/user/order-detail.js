const API_ORDERS = "http://localhost:5287/api/Orders";
const token = localStorage.getItem("token");

document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const orderId = params.get("id");
  const container = document.getElementById("orderDetail");

  if (!orderId) {
    container.innerHTML = `<div class="alert alert-danger">❌ Không tìm thấy đơn hàng</div>`;
    return;
  }

  try {
    const res = await fetch(`${API_ORDERS}/${orderId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) {
      const text = await res.text();
      console.error("Lỗi khi gọi API:", text);
      container.innerHTML = `<div class="alert alert-danger">❌ Không thể tải đơn hàng: ${text}</div>`;
      return;
    }

    const data = await res.json();

    const statusMap = {
      Pending: "🕐 Chờ xác nhận",
      Processing: "🔄 Đang xử lý",
      Shipped: "🚚 Đang giao",
      Delivered: "✅ Đã giao",
      Cancelled: "❌ Đã huỷ",
    };
    const statusText = statusMap[data.status] || data.status || "Không rõ";

    let itemsHtml = "";
    data.orderItems.forEach((item) => {
      itemsHtml += `
        <tr>
          <td>${item.productName}</td>
          <td>${item.quantity}</td>
          <td>${item.unitPrice.toLocaleString()} đ</td>
          <td>${(item.quantity * item.unitPrice).toLocaleString()} đ</td>
        </tr>`;
    });

    container.innerHTML = `
      <p><strong>👤 Tên khách hàng:</strong> ${data.customerName}</p>
      <p><strong>📞 Số điện thoại:</strong> ${data.phone}</p>
      <p><strong>📧 Email:</strong> ${data.email}</p>
      <p><strong>📍 Địa chỉ giao hàng:</strong> ${data.shippingAddress}</p>
      <p><strong>📦 Trạng thái:</strong> ${statusText}</p>
      <p><strong>🕒 Ngày đặt:</strong> ${new Date(
        data.createdAt
      ).toLocaleString()}</p>

      <table class="table table-bordered mt-3">
        <thead>
          <tr>
            <th>Sản phẩm</th>
            <th>Số lượng</th>
            <th>Đơn giá</th>
            <th>Thành tiền</th>
          </tr>
        </thead>
        <tbody>
          ${itemsHtml}
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3" class="text-end"><strong>Tổng cộng:</strong></td>
            <td><strong class="text-danger">${data.totalAmount.toLocaleString()} đ</strong></td>
          </tr>
        </tfoot>
      </table>
    `;
  } catch (err) {
    console.error("Lỗi:", err);
    container.innerHTML = `<div class="alert alert-danger">❌ Đã xảy ra lỗi khi tải dữ liệu đơn hàng.</div>`;
  }
});
