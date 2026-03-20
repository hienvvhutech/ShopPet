const API_PRODUCTS = `${API_BASE}/Products`;
const API_REVIEW = `${API_BASE}/reviews/product/`;
const API_CATEGORIES = `${API_BASE}/Categories`;

let allProducts = [];
let currentPage = 1;
const pageSize = 9;
document.addEventListener("DOMContentLoaded", async () => {
  await loadCategories();
  await loadProducts();
  attachEventListeners();
});

async function loadCategories() {
  const res = await fetch(`${API_CATEGORIES}/public`);
  if (!res.ok) return console.error("Lỗi khi gọi API category:", res.status);

  const categories = await res.json();
  const container = document.getElementById("dynamicCategories");
  container.innerHTML = `
    <div class="form-check">
      <input class="form-check-input" type="radio" name="category" value="" checked />
      <label class="form-check-label">Tất cả danh mục</label>
    </div>
  `;

  for (const cat of categories) {
    container.innerHTML += `
      <div class="form-check">
        <input class="form-check-input" type="radio" name="category" value="${cat.name}" />
        <label class="form-check-label">${cat.name}</label>
      </div>
    `;
  }

  document
    .querySelectorAll("input[name='category']")
    .forEach((el) => el.addEventListener("change", renderProducts));
}

async function loadProducts() {
  const token = localStorage.getItem("token");

  if (!token) {
    alert("Bạn chưa đăng nhập admin!");
    return;
  }

  const res = await fetch(API_PRODUCTS, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
  });

  if (!res.ok) {
    console.error("Lỗi load products:", res.status);
    alert("Lỗi khi tải dữ liệu");
    return;
  }

  allProducts = await res.json();

  for (const product of allProducts) {
    const reviewRes = await fetch(`${API_REVIEW}${product.id}`);
    const reviews = reviewRes.ok ? await reviewRes.json() : [];
    product.reviews = reviews;
    product.rating = reviews.length
      ? reviews.reduce((a, b) => a + (b.rating || 0), 0) / reviews.length
      : 0;
  }

  renderProducts();
}

function attachEventListeners() {
  document
    .getElementById("searchInput")
    ?.addEventListener("input", renderProducts);
  document
    .getElementById("sortSelect")
    ?.addEventListener("change", renderProducts);
  document
    .querySelectorAll("input[name='price']")
    .forEach((el) => el.addEventListener("change", renderProducts));
  document
    .querySelectorAll("input[name='rating']")
    .forEach((el) => el.addEventListener("change", renderProducts));
}

function resetFilters() {
  document.getElementById("searchInput").value = "";
  document.querySelector("input[name='category'][value='']").checked = true;
  document.querySelector("input[name='price'][value='']").checked = true;
  document.querySelector("input[name='rating'][value='']").checked = true;
  document.getElementById("sortSelect").value = "name";
  renderProducts();
}

function renderProducts() {
  const search = document.getElementById("searchInput").value.toLowerCase();
  const category =
    document.querySelector("input[name='category']:checked")?.value || "";
  const priceRange =
    document.querySelector("input[name='price']:checked")?.value || "";
  const minRating = parseFloat(
    document.querySelector("input[name='rating']:checked")?.value || "0"
  );
  const sort = document.getElementById("sortSelect").value;

  let filtered = [...allProducts].filter((p) => {
    const matchesSearch = p.name.toLowerCase().includes(search);
    const matchesCategory =
      !category || p.categoryName?.toLowerCase() === category.toLowerCase();
    const matchesRating = p.rating >= minRating;

    let matchesPrice = true;
    if (priceRange.includes("-")) {
      const [min, max] = priceRange.split("-").map(Number);
      matchesPrice = (!min || p.price >= min) && (!max || p.price <= max);
    }

    return matchesSearch && matchesCategory && matchesPrice && matchesRating;
  });

  if (sort === "priceAsc") filtered.sort((a, b) => a.price - b.price);
  else if (sort === "priceDesc") filtered.sort((a, b) => b.price - a.price);
  else if (sort === "rating") filtered.sort((a, b) => b.rating - a.rating);
  else filtered.sort((a, b) => a.name.localeCompare(b.name));

  const totalPages = Math.ceil(filtered.length / pageSize);
  if (currentPage > totalPages) currentPage = 1;

  const start = (currentPage - 1) * pageSize;
  const paginated = filtered.slice(start, start + pageSize);

  const container = document.getElementById("productList");
  container.innerHTML = paginated.length
    ? ""
    : '<div class="text-muted text-center p-4">Không có sản phẩm phù hợp.</div>';

  for (const p of paginated) {
    const imageUrl = p.imageUrl
      ? `http://localhost:5287${p.imageUrl}`
      : "https://via.placeholder.com/300x200";

    container.innerHTML += `
      <div class="col-md-4 mb-5">
        <div class="card h-100 pet-card">
          <img src="${imageUrl}" class="card-img-top" alt="${p.name}" />
          <div class="card-body p-3">
            <small class="text-muted text-uppercase">${
              p.categoryName || "Danh mục không rõ"
            }</small>
            <h5 class="card-title mt-2 text-pet-dark">${p.name}</h5>
            <p class="text-danger fw-bold fs-5 mb-3">${p.price.toLocaleString()}đ</p>
            <div class="rating-container d-flex align-items-center mb-3">
              <p class="text-warning mb-0 me-2">
                ${"★".repeat(Math.round(p.rating))}${"☆".repeat(
      5 - Math.round(p.rating)
    )}
                <small class="text-muted ms-1">(${p.reviews.length})</small>
              </p>
              <a href="review.html?productId=${
                p.id
              }" class="btn btn-link text-pet-accent">
                <i class="bi bi-star-fill"></i> Xem đánh giá
              </a>
            </div>
            <div class="d-flex justify-content-between">
              <a href="product-detail.html?id=${
                p.id
              }" class="btn btn-outline-pet btn-sm">
                <i class="bi bi-eye-fill"></i> Xem chi tiết
              </a>
              <button onclick="addToCart(${p.id}, '${p.name}', ${
      p.price
    })" class="btn btn-pet btn-sm">
                <i class="bi bi-cart-plus-fill"></i> Thêm vào giỏ
              </button>
            </div>
          </div>
        </div>
      </div>
    `;
  }

  renderPagination(totalPages);
}

function renderPagination(totalPages) {
  const container = document.getElementById("paginationContainer");
  container.innerHTML = "";

  if (totalPages <= 1) return;

  const addItem = (page, text, isActive = false, isDisabled = false) => {
    const li = document.createElement("li");
    li.className = `page-item ${isActive ? "active" : ""} ${
      isDisabled ? "disabled" : ""
    }`;
    li.innerHTML = `<a class="page-link" href="#">${text}</a>`;
    li.addEventListener("click", (e) => {
      e.preventDefault();
      if (!isDisabled) {
        currentPage = page;
        renderProducts();
      }
    });
    container.appendChild(li);
  };

  addItem(currentPage - 1, "«", false, currentPage === 1);
  for (let i = 1; i <= totalPages; i++) {
    addItem(i, i, i === currentPage);
  }
  addItem(currentPage + 1, "»", false, currentPage === totalPages);
}

function addToCart(productId, productName, unitPrice) {
  const token = localStorage.getItem("token");
  if (!token) {
    alert("Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng!");
    window.location.href = "../../login.html";
    return;
  }

  fetch("http://localhost:5287/api/Carts/items", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ productId, quantity: 1, unitPrice }),
  })
    .then((res) => {
      if (!res.ok) throw new Error("Thêm vào giỏ thất bại.");
      return res.json();
    })
    .then(() => {
      alert("Đã thêm sản phẩm '" + productName + "' vào giỏ hàng!");
      updateCartBadgeFromApi();
    })
    .catch((err) => {
      alert("Lỗi thêm sản phẩm: " + err.message);
      console.error(err);
    });
}

async function updateCartBadgeFromApi() {
  const badge = document.getElementById("cartCountBadge");
  const token = localStorage.getItem("token");
  if (!badge || !token) return;

  try {
    const res = await fetch("http://localhost:5287/api/Carts", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!res.ok) return;

    const data = await res.json();
    const cartItems = data.cartItems || [];
    const count = cartItems.reduce((sum, item) => sum + item.quantity, 0);

    if (count > 0) {
      badge.textContent = count;
      badge.classList.remove("d-none");
    } else {
      badge.classList.add("d-none");
    }
  } catch (err) {
    console.error("Lỗi khi cập nhật số giỏ hàng:", err);
  }
}
