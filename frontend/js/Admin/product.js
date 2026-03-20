const API_PRODUCTS = "http://localhost:5287/api/Products";
const API_CATEGORIES = "http://localhost:5287/api/Categories";
const token = localStorage.getItem("token");

if (!token) {
  alert("Vui lòng đăng nhập!");
  window.location.href = "/pages/login.html";
}

let fullProductList = [];
let filteredProductList = [];
let currentPage = 1;
const pageSize = 5;

if (location.pathname.includes("admin-product.html")) {
  document.addEventListener("DOMContentLoaded", () => {
    loadProducts();
    setupSearch();
  });
}

async function loadProducts() {
  const [productRes, categoryRes] = await Promise.all([
    fetch(API_PRODUCTS, { headers: { Authorization: `Bearer ${token}` } }),
    fetch(API_CATEGORIES, { headers: { Authorization: `Bearer ${token}` } }),
  ]);

  if (!productRes.ok || !categoryRes.ok) {
    alert("Lỗi khi tải dữ liệu");
    return;
  }

  fullProductList = await productRes.json();
  const categories = await categoryRes.json();
  const categoryMap = {};
  categories.forEach((c) => (categoryMap[c.id] = c.name));

  filteredProductList = fullProductList;
  renderProductList(categoryMap);
}

function setupSearch() {
  const input = document.getElementById("searchProductInput");
  if (!input) return;

  input.addEventListener("input", function () {
    const keyword = this.value.toLowerCase().trim();
    currentPage = 1;
    filteredProductList = fullProductList.filter((p) =>
      p.name.toLowerCase().includes(keyword)
    );
    renderProductList();
  });
}

function renderProductList(categoryMap = {}) {
  const tbody = document.getElementById("productTableBody");
  tbody.innerHTML = "";

  const pageData = paginate(filteredProductList, currentPage, pageSize);
  if (pageData.length === 0) {
    tbody.innerHTML = `<tr><td colspan="8" class="text-center">Không có sản phẩm</td></tr>`;
    return;
  }

  pageData.forEach((p, index) => {
    tbody.innerHTML += `
      <tr>
        <td>${(currentPage - 1) * pageSize + index + 1}</td>
        <td>${p.name}</td>
        <td>${categoryMap[p.categoryId] || "Không rõ"}</td>
        <td>${p.price.toLocaleString()} đ</td>
        <td>${p.description || ""}</td>
        <td><img src="http://localhost:5287${p.imageUrl}" width="60" /></td>
        <td>${p.stockQuantity}</td>
        <td>
          <a href="admin-product-edit.html?id=${
            p.id
          }" class="btn btn-warning btn-sm">Sửa</a>
          <button onclick="deleteProduct(${
            p.id
          })" class="btn btn-danger btn-sm">Xoá</button>
        </td>
      </tr>
    `;
  });

  renderPagination();
}

function paginate(array, page, size) {
  const start = (page - 1) * size;
  return array.slice(start, start + size);
}

function renderPagination() {
  const pagination = document.getElementById("pagination");
  const totalPages = Math.ceil(filteredProductList.length / pageSize);
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
        renderProductList();
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

async function loadCategoriesToSelect(selectId) {
  const res = await fetch(API_CATEGORIES, {
    headers: { Authorization: `Bearer ${token}` },
  });

  if (!res.ok) {
    alert("Không thể tải danh mục");
    return;
  }

  const categories = await res.json();
  const select = document.getElementById(selectId);
  if (!select) return;
  select.innerHTML = "";
  categories.forEach((c) => {
    const opt = document.createElement("option");
    opt.value = c.id;
    opt.textContent = c.name;
    select.appendChild(opt);
  });
}

async function deleteProduct(id) {
  if (!confirm("Bạn có chắc chắn muốn xoá sản phẩm này?")) return;

  const res = await fetch(`${API_PRODUCTS}/${id}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  });

  if (res.ok) {
    alert("Đã xoá sản phẩm!");
    loadProducts();
  } else {
    alert("Lỗi khi xoá sản phẩm");
  }
}

if (location.pathname.includes("admin-product-add.html")) {
  const form = document.getElementById("addProductForm");
  loadCategoriesToSelect("categorySelect");

  form?.addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new FormData();
    formData.append("Name", document.getElementById("name").value);
    formData.append(
      "Description",
      document.getElementById("description").value
    );
    formData.append("Price", document.getElementById("price").value);
    formData.append(
      "StockQuantity",
      document.getElementById("stockQuantity").value
    );
    formData.append(
      "CategoryId",
      document.getElementById("categorySelect").value
    );

    const imageFile = document.getElementById("image").files[0];
    if (imageFile) formData.append("image", imageFile);

    const res = await fetch(API_PRODUCTS, {
      method: "POST",
      headers: { Authorization: `Bearer ${token}` },
      body: formData,
    });

    const data = await res.json();
    if (res.ok) {
      alert("Đã thêm sản phẩm!");
      window.location.href = "admin-product.html";
    } else {
      alert(data.message || "Lỗi khi thêm sản phẩm");
    }
  });
}

if (location.pathname.includes("admin-product-edit.html")) {
  const id = new URLSearchParams(location.search).get("id");
  const form = document.getElementById("editProductForm");

  loadCategoriesToSelect("categorySelect");

  async function loadProduct() {
    const res = await fetch(`${API_PRODUCTS}/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const p = await res.json();
    document.getElementById("name").value = p.name;
    document.getElementById("description").value = p.description;
    document.getElementById("price").value = p.price;
    document.getElementById("stockQuantity").value = p.stockQuantity;
    document.getElementById("categorySelect").value = p.categoryId;
    document.getElementById("currentImage").src =
      "http://localhost:5287" + p.imageUrl;
  }

  form?.addEventListener("submit", async (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("Name", document.getElementById("name").value);
    formData.append(
      "Description",
      document.getElementById("description").value
    );
    formData.append("Price", document.getElementById("price").value);
    formData.append(
      "StockQuantity",
      document.getElementById("stockQuantity").value
    );
    formData.append(
      "CategoryId",
      document.getElementById("categorySelect").value
    );
    const imageFile = document.getElementById("image").files[0];
    if (imageFile) {
      formData.append("image", imageFile);
    } else {
      const currentImageSrc = document
        .getElementById("currentImage")
        .getAttribute("src");
      const relativeUrl = currentImageSrc.replace("http://localhost:5287", "");
      formData.append("ImageUrl", relativeUrl);
    }

    const res = await fetch(`${API_PRODUCTS}/${id}`, {
      method: "PUT",
      headers: { Authorization: `Bearer ${token}` },
      body: formData,
    });

    const data = await res.json();
    if (res.ok) {
      alert("Cập nhật sản phẩm thành công!");
      window.location.href = "admin-product.html";
    } else {
      alert(data.message || "Lỗi cập nhật sản phẩm");
    }
  });

  loadProduct();
}
