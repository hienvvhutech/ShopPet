const API_URL = "http://localhost:5287/api/users/staff";
const token = localStorage.getItem("token");

function checkAuth() {
  if (!token) {
    alert("Vui lòng đăng nhập.");
    window.location.href = "/pages/admin/login.html";
  }
}

let fullStaffList = [];
let filteredStaffList = [];
let currentPage = 1;
const pageSize = 5;

async function fetchStaffList() {
  const tbody = document.getElementById("staffTableBody");
  try {
    const response = await fetch(API_URL, {
      headers: { Authorization: "Bearer " + token },
    });

    if (!response.ok) throw new Error("Không thể tải danh sách");

    fullStaffList = await response.json();
    renderStaffList(fullStaffList);
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-danger text-center">${err.message}</td></tr>`;
  }
}

function paginate(array, page, size) {
  const start = (page - 1) * size;
  return array.slice(start, start + size);
}

function renderStaffList(list) {
  filteredStaffList = list;
  const tbody = document.getElementById("staffTableBody");
  tbody.innerHTML = "";

  const pageData = paginate(filteredStaffList, currentPage, pageSize);

  if (pageData.length === 0) {
    tbody.innerHTML = `<tr><td colspan="5" class="text-center">Không tìm thấy nhân viên</td></tr>`;
    return;
  }

  pageData.forEach((staff, index) => {
    const row = document.createElement("tr");
    row.innerHTML = `
      <td>${(currentPage - 1) * pageSize + index + 1}</td>
      <td>${staff.fullName || "(Chưa có tên)"}</td>
      <td>${staff.email}</td>
      <td>${staff.address || ""}</td>
      <td>
        <a href="admin-staff-edit.html?id=${
          staff.id
        }" class="btn btn-sm btn-warning">✏ Sửa</a>
        <button class="btn btn-sm btn-danger ms-2" onclick="deleteStaff('${
          staff.id
        }')">🗑 Xoá</button>
      </td>
    `;
    tbody.appendChild(row);
  });

  renderPagination();
}

function renderPagination() {
  const totalPages = Math.ceil(filteredStaffList.length / pageSize);
  const pagination = document.getElementById("pagination");
  pagination.innerHTML = "";

  if (totalPages <= 1) return;

  const createItem = (label, page, disabled = false, active = false) => {
    const li = document.createElement("li");
    li.className = `page-item ${disabled ? "disabled" : ""} ${
      active ? "active" : ""
    }`;
    li.innerHTML = `<button class="page-link">${label}</button>`;
    if (!disabled && !active) {
      li.addEventListener("click", () => {
        currentPage = page;
        renderStaffList(filteredStaffList);
      });
    }
    return li;
  };

  pagination.appendChild(createItem("«", 1, currentPage === 1));
  pagination.appendChild(createItem("‹", currentPage - 1, currentPage === 1));
  for (let i = 1; i <= totalPages; i++) {
    pagination.appendChild(createItem(i, i, false, i === currentPage));
  }
  pagination.appendChild(
    createItem("›", currentPage + 1, currentPage === totalPages)
  );
  pagination.appendChild(
    createItem("»", totalPages, currentPage === totalPages)
  );
}

document.getElementById("searchInput")?.addEventListener("input", function () {
  const keyword = this.value.toLowerCase();
  currentPage = 1;
  const filtered = fullStaffList.filter(
    (staff) =>
      staff.fullName?.toLowerCase().includes(keyword) ||
      staff.email?.toLowerCase().includes(keyword)
  );
  renderStaffList(filtered);
});

async function handleAddStaff() {
  const form = document.getElementById("addStaffForm");
  if (!form) return;

  form.addEventListener("submit", async function (e) {
    e.preventDefault();

    const data = {
      email: form.email.value,
      password: form.password.value,
      confirmPassword: form.confirmPassword.value,
      fullName: form.fullName.value,
      address: form.address.value,
    };

    try {
      const res = await fetch(API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
        body: JSON.stringify(data),
      });

      const result = await res.json();
      alert(result.message || result.error);
      if (res.ok) {
        window.location.href = "admin-staff.html";
      }
    } catch (err) {
      alert("Lỗi khi tạo tài khoản: " + err.message);
    }
  });
}

async function handleEditStaff() {
  const form = document.getElementById("editStaffForm");
  if (!form) return;

  const urlParams = new URLSearchParams(window.location.search);
  const userId = urlParams.get("id");
  if (!userId) return;

  try {
    const res = await fetch(API_URL, {
      headers: { Authorization: "Bearer " + token },
    });
    const users = await res.json();
    const user = users.find((u) => u.id === userId);

    if (!user) {
      alert("Không tìm thấy nhân viên");
      window.location.href = "admin-staff.html";
      return;
    }

    form.email.value = user.email;
    form.fullName.value = user.fullName || "";
    form.address.value = user.address || "";

    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      const data = {
        email: form.email.value,
        fullName: form.fullName.value,
        address: form.address.value,
      };

      const updateRes = await fetch(`${API_URL}/${userId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
        body: JSON.stringify(data),
      });

      const result = await updateRes.json();
      alert(result.message || result.error);
      if (updateRes.ok) window.location.href = "admin-staff.html";
    });
  } catch (err) {
    alert("Lỗi khi tải nhân viên: " + err.message);
  }
}

async function deleteStaff(userId) {
  if (!confirm("Bạn có chắc chắn muốn xoá nhân viên này?")) return;

  try {
    const res = await fetch(`${API_URL}/${userId}`, {
      method: "DELETE",
      headers: { Authorization: "Bearer " + token },
    });

    const result = await res.json();
    alert(result.message);
    if (res.ok) fetchStaffList();
  } catch (err) {
    alert("Lỗi khi xoá: " + err.message);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  checkAuth();
  fetchStaffList();
  handleAddStaff();
  handleEditStaff();
});
