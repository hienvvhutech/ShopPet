const API_SUMMARY = "http://localhost:5287/api/dashboard/summary";
const API_ORDER_STATUS = "http://localhost:5287/api/dashboard/order-status";

document.addEventListener("DOMContentLoaded", async () => {
  const user = JSON.parse(localStorage.getItem("user"));
  if (!user || !user.token || !user.roles?.includes("Admin")) {
    alert("Bạn cần đăng nhập bằng tài khoản Admin!");
    window.location.href = "../pages/login.html";
    return;
  }

  await Promise.all([
    loadSummary(user.token),
    loadOrderStatusChart(user.token),
  ]);
});

async function loadSummary(token) {
  try {
    const res = await fetch(API_SUMMARY, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Không thể lấy dữ liệu thống kê.");

    const data = await res.json();
    console.log("📊 Dữ liệu tổng quan:", data);

    document.getElementById("summaryContainer").innerHTML = `
      <div class="col-md-3">
        <div class="card border-primary shadow text-center p-3">
          <h4 class="text-primary">${data.totalOrders}</h4>
          <p>Đơn hàng</p>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-success shadow text-center p-3">
          <h4 class="text-success">${data.totalUsers}</h4>
          <p>Người dùng</p>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-warning shadow text-center p-3">
          <h4 class="text-warning">${data.totalProducts}</h4>
          <p>Sản phẩm</p>
        </div>
      </div>
      <div class="col-md-3">
        <div class="card border-danger shadow text-center p-3">
          <h4 class="text-danger">${data.totalRevenue.toLocaleString()}đ</h4>
          <p>Doanh thu</p>
        </div>
      </div>
    `;

    renderRevenueChart(data.monthlyRevenues);
  } catch (err) {
    console.error("Lỗi tải thống kê:", err);
    alert("Không thể tải dữ liệu tổng quan. Vui lòng thử lại.");
  }
}

function renderRevenueChart(monthlyRevenues) {
  const ctx = document.getElementById("revenueChart").getContext("2d");

  const labels = monthlyRevenues.map((r) => r.month);
  const values = monthlyRevenues.map((r) => r.revenue / 1_000_000); // Đổi về triệu

  new Chart(ctx, {
    type: "bar",
    data: {
      labels,
      datasets: [
        {
          label: "Doanh thu (triệu đồng)",
          data: values,
          backgroundColor: "rgba(54, 162, 235, 0.6)",
        },
      ],
    },
    options: {
      responsive: true,
      plugins: { legend: { position: "top" } },
      scales: { y: { beginAtZero: true } },
    },
  });
}

async function loadOrderStatusChart(token) {
  try {
    const res = await fetch(API_ORDER_STATUS, {
      headers: { Authorization: `Bearer ${token}` },
    });
    if (!res.ok) throw new Error("Không thể lấy dữ liệu trạng thái đơn hàng.");

    const stats = await res.json();
    const container = document.getElementById("orderStatusStatsText");
    container.innerHTML = `
  <p>🟢 Mới: ${stats.new}</p>
  <p>🟡 Đang xử lý: ${stats.processing}</p>
  <p>🔵 Đã giao: ${stats.shipped}</p>
  <p>🔴 Đã huỷ: ${stats.cancelled}</p>
`;

    const ctx = document.getElementById("orderStatusChart").getContext("2d");
    new Chart(ctx, {
      type: "doughnut",
      data: {
        labels: ["Mới", "Đang xử lý", "Đã giao", "Đã huỷ"],
        datasets: [
          {
            data: [stats.new, stats.processing, stats.shipped, stats.cancelled],
            backgroundColor: ["#28a745", "#ffc107", "#007bff", "#dc3545"],
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: "bottom" },
        },
      },
    });
  } catch (err) {
    console.error("Lỗi trạng thái đơn hàng:", err);
  }
}
