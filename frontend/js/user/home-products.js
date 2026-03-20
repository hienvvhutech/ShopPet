const API_PRODUCTS = "http://localhost:5287/api/Products";
const API_REVIEW = "http://localhost:5287/api/reviews/product/";

document.addEventListener("DOMContentLoaded", async () => {
  const container = document.getElementById("featuredProducts");
  if (!container) return;

  const res = await fetch(API_PRODUCTS);
  if (!res.ok) return console.error("Lỗi tải sản phẩm");

  let products = await res.json();
  products = products.slice(0, 4);

  for (const p of products) {
    const reviewRes = await fetch(`${API_REVIEW}${p.id}`);
    const reviews = reviewRes.ok ? await reviewRes.json() : [];
    const rating = reviews.length
      ? reviews.reduce((sum, r) => sum + r.rating, 0) / reviews.length
      : 0;

    const imageUrl = p.imageUrl
      ? `http://localhost:5287${p.imageUrl}`
      : "https://via.placeholder.com/300x200";

    container.innerHTML += `
  <div class="col-md-3 mb-4">
    <div class="card h-100 shadow-sm">
      <img src="${imageUrl}" class="card-img-top" alt="${p.name}" />
      <div class="card-body">
        <h6 class="text-muted mb-1">Danh mục: ${p.categoryName}</h6>
        <h5 class="card-title">${p.name}</h5>
        <p class="text-danger fw-bold">${p.price.toLocaleString()}đ</p>
        <p class="text-secondary mb-2">Mô tả ngắn: Sản phẩm chất lượng cao cho thú cưng.</p>
        <div class="rating-container">
          <p class="text-warning mb-2">
            ${"★".repeat(Math.round(rating))}${"☆".repeat(
      5 - Math.round(rating)
    )}
            <small class="text-muted">(${reviews.length})</small>
          </p>
          <a href="review.html?productId=${
            p.id
          }" class="btn btn-link">Xem đánh giá</a>
        </div>
        <div class="d-flex gap-2">
          <a href="product-detail.html?id=${
            p.id
          }" class="btn btn-outline-secondary btn-sm">
            <i class="bi bi-eye"></i> Xem chi tiết
          </a>
          <button onclick="addToCart(${p.id}, '${p.name}', ${
      p.price
    })" class="btn btn-pink btn-sm"> Thêm vào giỏ hàng </button>

        </div>
      </div>
    </div>
  </div>
`;
  }
  window.addToCart = function (productId, productName, unitPrice) {
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
  };

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
});
