const API_ORDERS = "http://localhost:5287/api/Orders";
const token = localStorage.getItem("token");

async function loadOrders() {
  if (!token) {
    alert("Bạn cần đăng nhập để xem đơn hàng!");
    window.location.href = "../login.html";
    return;
  }

  const res = await fetch(API_ORDERS, {
    headers: { Authorization: `Bearer ${token}` },
  });

  const orders = await res.json();
  const container = document.getElementById("orderList");
  container.innerHTML = "";

  if (!orders.length) {
    container.innerHTML = `<div class="alert alert-info">Bạn chưa có đơn hàng nào.</div>`;
    return;
  }

  const statusMap = {
    Pending: "🕐 Chờ xác nhận",
    Processing: "🔄 Đang xử lý",
    Shipped: "🚚 Đang giao",
    Delivered: "✅ Đã giao",
    Cancelled: "❌ Đã huỷ",
  };
  orders.forEach((order) => {
    const statusText = statusMap[order.status] || order.status || "Không rõ";

    container.innerHTML += `
      <div class="card mb-3">
        <div class="card-body">
          <h5 class="card-title">🛒 Mã đơn hàng: ${order.id}</h5>
          <p class="card-text">Ngày đặt: ${new Date(
            order.createdAt
          ).toLocaleDateString()}</p>
          <p class="card-text">Trạng thái: <strong>${statusText}</strong></p>
          <p class="card-text">Tổng tiền: <strong class="text-danger">${order.totalAmount.toLocaleString()} đ</strong></p>
          <a href="order-detail.html?id=${
            order.id
          }" class="btn btn-outline-primary btn-sm">🔍 Xem chi tiết</a>
        </div>
      </div>
    `;
  });
}

document.addEventListener("DOMContentLoaded", loadOrders);
