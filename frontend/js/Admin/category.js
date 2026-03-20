const API_URL = "http://localhost:5287/api/Categories";
const token = localStorage.getItem("token");

if (!token) {
  alert("Vui lòng đăng nhập!");
  window.location.href = "/pages/login.html";
}

let fullCategoryList = [];
let filteredCategoryList = [];
let currentPage = 1;
const pageSize = 5;

if (location.pathname.includes("admin-category.html")) {
  document.addEventListener("DOMContentLoaded", () => {
    loadCategories();
    setupSearch();
  });
}

async function loadCategories() {
  const tbody = document.getElementById("categoryTableBody");
  if (!tbody) return;

  try {
    const res = await fetch(API_URL, {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) throw new Error("Không thể load danh mục");

    fullCategoryList = await res.json();
    filteredCategoryList = fullCategoryList;
    renderCategoryList();
  } catch (err) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-danger text-center">${err.message}</td></tr>`;
  }
}

function setupSearch() {
  const input = document.getElementById("searchInput");
  if (!input) return;

  input.addEventListener("input", function () {
    const keyword = this.value
      .toLowerCase()
      .normalize("NFD")
      .replace(/\u0300-\u036f/g, "")
      .trim();

    currentPage = 1;
    filteredCategoryList = fullCategoryList.filter((cat) => {
      const text = `${cat.name} ${cat.description || ""}`
        .toLowerCase()
        .normalize("NFD")
        .replace(/\u0300-\u036f/g, "");
      return keyword.split(" ").every((kw) => text.includes(kw));
    });

    renderCategoryList();
  });
}

function renderCategoryList() {
  const tbody = document.getElementById("categoryTableBody");
  if (!tbody) return;

  tbody.innerHTML = "";

  const pageData = paginate(filteredCategoryList, currentPage, pageSize);

  if (pageData.length === 0) {
    tbody.innerHTML = `<tr><td colspan="4" class="text-center">Không có danh mục nào</td></tr>`;
    return;
  }

  pageData.forEach((cat, index) => {
    tbody.innerHTML += `
      <tr>
        <td>${(currentPage - 1) * pageSize + index + 1}</td>
        <td>${cat.name}</td>
        <td>${cat.description || ""}</td>
        <td>
          <a href="admin-category-edit.html?id=${
            cat.id
          }" class="btn btn-warning btn-sm">✏ Sửa</a>
          <button class="btn btn-danger btn-sm" onclick="deleteCategory(${
            cat.id
          })">🗑 Xoá</button>
        </td>
      </tr>`;
  });

  renderPagination();
}

function paginate(array, page, size) {
  const start = (page - 1) * size;
  return array.slice(start, start + size);
}

function renderPagination() {
  const pagination = document.getElementById("pagination");
  if (!pagination) return;

  const totalPages = Math.ceil(filteredCategoryList.length / pageSize);
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
        renderCategoryList();
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

async function deleteCategory(id) {
  if (!confirm("Bạn chắc chắn muốn xoá?")) return;

  try {
    const res = await fetch(`${API_URL}/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });

    const result = await res.json();
    alert(result.message || "Đã xoá danh mục!");
    if (res.ok) loadCategories();
  } catch (err) {
    alert("Lỗi xoá danh mục");
  }
}

if (location.pathname.includes("admin-category-add.html")) {
  document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("addCategoryForm");
    if (!form) return;

    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      const name = document.getElementById("categoryName").value.trim();
      const description = document
        .getElementById("categoryDescription")
        .value.trim();

      const res = await fetch(API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ name, description }),
      });

      const data = await res.json();
      alert(data.message || "Thêm thành công!");
      if (res.ok) window.location.href = "admin-category.html";
    });
  });
}

if (location.pathname.includes("admin-category-edit.html")) {
  document.addEventListener("DOMContentLoaded", () => {
    const id = new URLSearchParams(location.search).get("id");
    if (!id) {
      alert("Thiếu ID danh mục");
      window.location.href = "admin-category.html";
      return;
    }

    const form = document.getElementById("editCategoryForm");
    const nameInput = document.getElementById("editCategoryName");
    const descInput = document.getElementById("editCategoryDescription");
    const alertBox = document.getElementById("alertBox");

    fetch(`${API_URL}/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    })
      .then((res) => res.json())
      .then((data) => {
        nameInput.value = data.name || "";
        descInput.value = data.description || "";
      })
      .catch(() => {
        alert("Không thể tải thông tin danh mục");
        window.location.href = "admin-category.html";
      });

    form.addEventListener("submit", async (e) => {
      e.preventDefault();

      const updateData = {
        name: nameInput.value.trim(),
        description: descInput.value.trim(),
      };

      const res = await fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(updateData),
      });

      const data = await res.json();
      alertBox.textContent = data.message || "Cập nhật thành công!";
      alertBox.className = "alert alert-success";
      alertBox.classList.remove("d-none");
      if (res.ok)
        setTimeout(() => (window.location.href = "admin-category.html"), 1000);
    });
  });
}
