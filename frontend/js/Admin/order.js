const API_ORDERS = "http://localhost:5287/api/orders/admin";
const token = localStorage.getItem("token");

document.addEventListener("DOMContentLoaded", loadOrders);

async function loadOrders() {
  const tbody = document.getElementById("orderTableBody");
  const message = document.getElementById("message");
  tbody.innerHTML = "";
  message.innerText = "";

  try {
    const res = await fetch(API_ORDERS, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    if (res.status === 403) {
      message.innerText = "Bạn không có quyền truy cập chức năng này.";
      return;
    }

    if (!res.ok) {
      message.innerText = "Không thể tải danh sách đơn hàng.";
      return;
    }

    const data = await res.json();

    if (data.length === 0) {
      tbody.innerHTML = `<tr><td colspan="7" class="text-center">Không có đơn hàng nào.</td></tr>`;
      return;
    }

    data.forEach((order) => {
      const tr = document.createElement("tr");

      const statusMap = {
        Pending: "Chờ xử lý",
        Processing: "Đang xử lý",
        Shipped: "Đã gửi hàng",
        Delivered: "Đã giao hàng",
        Cancelled: "Đã hủy",
      };

      const statusOptions = Object.entries(statusMap)
        .map(
          ([value, label]) =>
            `<option value="${value}" ${
              order.status === value ? "selected" : ""
            }>${label}</option>`
        )
        .join("");

      tr.innerHTML = `
        <td>${order.id}</td>
        <td>${order.customerName}</td>
        <td>${order.phone}</td>
        <td>${order.totalAmount.toLocaleString()}đ</td>
        <td>
          <select onchange="updateStatus(${
            order.id
          }, this.value)" class="form-select form-select-sm">
            ${statusOptions}
          </select>
        </td>
        <td>${formatDate(order.createdAt)}</td>
        <td>${order.updatedAt ? formatDate(order.updatedAt) : "–"}</td>
      `;

      tbody.appendChild(tr);
    });
  } catch (err) {
    console.error("Lỗi khi tải đơn hàng:", err);
    message.innerText = "Đã xảy ra lỗi khi kết nối đến máy chủ.";
  }
}

async function updateStatus(orderId, newStatus) {
  const confirmChange = confirm(
    `Xác nhận cập nhật đơn hàng ${orderId} sang trạng thái "${newStatus}"?`
  );
  if (!confirmChange) return;

  const res = await fetch(
    `http://localhost:5287/api/orders/${orderId}/status`,
    {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ status: newStatus }),
    }
  );

  const result = await res.json();
  if (res.ok) {
    alert("Cập nhật trạng thái thành công!");
    loadOrders();
  } else {
    alert("Lỗi: " + result.message);
  }
}

function formatDate(dateStr) {
  const d = new Date(dateStr);
  return `${d.getDate()}/${
    d.getMonth() + 1
  }/${d.getFullYear()} ${d.getHours()}:${d.getMinutes()}`;
}
