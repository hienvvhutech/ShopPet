const API_PRODUCTS = "http://localhost:5287/api/Products";
const API_CATEGORIES = "http://localhost:5287/api/Categories";
const token = localStorage.getItem("token");

document.addEventListener("DOMContentLoaded", async () => {
  const params = new URLSearchParams(window.location.search);
  const productId = params.get("id");
  if (!productId) {
    document.getElementById("productDetail").innerHTML =
      "<div class='alert alert-danger'>Không tìm thấy sản phẩm</div>";
    return;
  }

  try {
    const res = await fetch(`${API_PRODUCTS}/${productId}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.message || "Không thể tải sản phẩm");

    let categoryName = "Chưa rõ";
    if (data.category?.name) {
      categoryName = data.category.name;
    } else if (data.categoryId) {
      const catRes = await fetch(`${API_CATEGORIES}/${data.categoryId}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (catRes.ok) {
        const cat = await catRes.json();
        categoryName = cat.name;
      }
    }

    const rating = data.rating ?? 5;
    const reviewCount = data.reviewCount ?? 0;
    const starsHtml = renderStars(rating);

    document.getElementById("productDetail").innerHTML = `
      <!-- Khối trên: ảnh và thông tin -->
      <div class="row g-4 mb-4">
        <div class="col-md-4">
          <img src="http://localhost:5287${data.imageUrl}" 
               class="img-fluid rounded shadow-sm" 
               onerror="this.src='/images/default.jpg'" />
        </div>
        <div class="col-md-8">
          <h2>${data.name}</h2>
          <div class="text-warning mb-2">
            ${starsHtml} <small class="text-muted">(${reviewCount} đánh giá)</small>
          </div>
          <p><strong>Giá:</strong> ${data.price.toLocaleString()}đ</p>
          <p><strong>Danh mục:</strong> ${categoryName}</p>
          <div class="mb-3">
            <a href="review.html?productId=${
              data.id
            }" class="btn btn-outline-secondary me-2">
              📝 Xem đánh giá
            </a>
            <button class="btn btn-primary" onclick="addToCart(${data.id}, '${
      data.name
    }', ${data.price})">
              🛒 Thêm vào giỏ
            </button>
          </div>
        </div>
      </div>

      <!-- Khối dưới: mô tả và thông tin -->
      <div class="row">
        <!-- Mô tả chi tiết -->
        <div class="col-lg-8">
          <div class="border rounded p-3 mb-3 bg-white shadow-sm">
            <h5><i class="bi bi-info-circle me-1"></i> Mô tả chi tiết sản phẩm</h5>
            <p>${data.description || "Không có mô tả chi tiết."}</p>
          </div>
        </div>

        <!-- Thông tin vận chuyển + cam kết -->
        <div class="col-lg-4">
          <div class="border rounded p-3 mb-3 bg-white shadow-sm">
            <h6><i class="bi bi-truck me-1"></i> Thông tin vận chuyển</h6>
            <div class="d-flex justify-content-between"><span>Miễn phí:</span><strong>Đơn từ 500,000đ</strong></div>
            <div class="d-flex justify-content-between"><span>Giao hàng:</span><strong>1–3 ngày</strong></div>
            <div class="d-flex justify-content-between"><span>Đổi trả:</span><strong>Trong 7 ngày</strong></div>
          </div>

          <div class="border rounded p-3 bg-white shadow-sm">
            <h6><i class="bi bi-shield-check me-1"></i> Cam kết chất lượng</h6>
            <ul class="mb-0 ps-3">
              <li>100% hàng chính hãng</li>
              <li>Kiểm định chất lượng kỹ lưỡng</li>
              <li>Đảm bảo an toàn cho thú cưng</li>
              <li>Hỗ trợ khách hàng 24/7</li>
            </ul>
          </div>
        </div>
      </div>
    `;

    window.productId = data.id;
  } catch (err) {
    document.getElementById("productDetail").innerHTML = `
      <div class="alert alert-danger">${err.message}</div>`;
  }
});

function renderStars(rating) {
  const fullStars = Math.round(rating);
  let html = "";
  for (let i = 1; i <= 5; i++) {
    html += `<i class="bi ${
      i <= fullStars ? "bi-star-fill" : "bi-star"
    } text-warning me-1"></i>`;
  }
  return html;
}

function addToCart(productId, productName, unitPrice) {
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
      alert("Đã thêm sản phẩm " + productName + " vào giỏ hàng!");
      updateCartBadgeFromApi();
    })
    .catch((err) => {
      alert(err.message);
      console.error(err);
    });

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
}
