const API_CUSTOMERS = "http://localhost:5287/api/users/customers";
const token = localStorage.getItem("token");

let customers = [];
let allCustomers = [];
let currentPage = 1;
const pageSize = 5;

document.addEventListener("DOMContentLoaded", async () => {
  await loadCustomers();
  renderPagination();
});

async function loadCustomers() {
  const tbody = document.getElementById("userTableBody");
  const message = document.getElementById("message");
  tbody.innerHTML = "";
  message.innerText = "";

  try {
    const res = await fetch(API_CUSTOMERS, {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) {
      message.innerText = "Không thể tải danh sách khách hàng.";
      return;
    }

    customers = await res.json();
    allCustomers = customers;
    renderTable();
    renderPagination();
  } catch (err) {
    console.error("Lỗi:", err);
    message.innerText = "Lỗi kết nối đến máy chủ.";
  }
}

function renderTable(filteredList = allCustomers) {
  const tbody = document.getElementById("userTableBody");
  const searchTerm = document.getElementById("searchInput").value.toLowerCase();

  const filtered = filteredList.filter((c) => {
    const email = c.email?.toLowerCase() || "";
    const name = c.fullName?.toLowerCase() || "";
    return email.includes(searchTerm) || name.includes(searchTerm);
  });

  const start = (currentPage - 1) * pageSize;
  const paginated = filtered.slice(start, start + pageSize);

  tbody.innerHTML = paginated.length
    ? paginated
        .map(
          (u, i) => `
        <tr>
          <td>${start + i + 1}</td>
          <td>${u.email}</td>
          <td>${u.fullName || "–"}</td>
          <td>${u.address || "–"}</td>
          <td>${u.lockoutEnd ? "🔒 Đã khoá" : "✅ Bình thường"}</td>
          <td>
            <button class="btn btn-sm ${
              u.lockoutEnd ? "btn-success" : "btn-danger"
            }" onclick="toggleLock('${u.id}', ${!!u.lockoutEnd})">
              ${u.lockoutEnd ? "Mở khoá" : "Khoá"}
            </button>
          </td>
        </tr>
      `
        )
        .join("")
    : `<tr><td colspan='6' class='text-center'>Không có kết quả.</td></tr>`;
}

function handleSearch() {
  const keyword = document.getElementById("searchInput").value.toLowerCase();

  const filtered = allCustomers.filter((c) => {
    const email = c.email?.toLowerCase() || "";
    const name = c.fullName?.toLowerCase() || "";
    return email.includes(keyword) || name.includes(keyword);
  });

  currentPage = 1;
  renderTable(filtered);
  renderPagination(filtered.length);
}

function renderPagination() {
  const container = document.getElementById("pagination");
  const searchTerm = document.getElementById("searchInput").value.toLowerCase();
  const filteredCount = customers.filter((c) =>
    c.email.toLowerCase().includes(searchTerm)
  ).length;

  const totalPages = Math.ceil(filteredCount / pageSize);
  if (totalPages <= 1) return (container.innerHTML = "");

  container.innerHTML = "";

  for (let i = 1; i <= totalPages; i++) {
    const btn = document.createElement("button");
    btn.className = `btn btn-sm ${
      i === currentPage ? "btn-primary" : "btn-outline-primary"
    } mx-1`;
    btn.textContent = i;
    btn.onclick = () => {
      currentPage = i;
      renderTable();
      renderPagination();
    };
    container.appendChild(btn);
  }
}

async function toggleLock(userId, isLocked) {
  const confirmText = isLocked ? "mở khoá" : "khoá";
  if (!confirm(`Bạn có chắc chắn muốn ${confirmText} tài khoản này?`)) return;

  try {
    const res = await fetch(
      `http://localhost:5287/api/users/customers/${userId}/lock?lockAccount=${!isLocked}`,
      {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const result = await res.json();
    if (res.ok) {
      alert("Cập nhật trạng thái thành công!");
      await loadCustomers();
    } else {
      alert("Lỗi: " + result.message);
    }
  } catch (err) {
    alert("Lỗi kết nối.");
    console.error(err);
  }
}
