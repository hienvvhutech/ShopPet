const API_CART = "http://localhost:5287/api/Carts";
const token = localStorage.getItem("token");

async function loadCart() {
  if (!token) {
    alert("Bạn cần đăng nhập để xem giỏ hàng!");
    window.location.href = "../login.html";
    return;
  }

  const res = await fetch(API_CART, {
    headers: { Authorization: `Bearer ${token}` },
  });

  const data = await res.json();
  const container = document.getElementById("cartContainer");
  container.innerHTML = "";

  if (!data.cartItems || data.cartItems.length === 0) {
    container.innerHTML = "<div class='alert alert-info'>Giỏ hàng trống!</div>";
    updateCartBadgeFromApi();
    document.getElementById("subtotalText").innerText = "0 đ";
    document.getElementById("totalText").innerText = "0 đ";
    return;
  }

  let subtotal = 0;

  for (const item of data.cartItems) {
    const imageUrl = item.productImage
      ? `http://localhost:5287${item.productImage}`
      : "https://via.placeholder.com/100";

    const itemTotal = item.unitPrice * item.quantity;
    subtotal += itemTotal;

    container.innerHTML += `
      <div class="bg-white p-3 rounded shadow-sm mb-3 d-flex align-items-center gap-3">
        <img src="${imageUrl}" class="rounded" width="80" height="80" style="object-fit: cover;" />
        <div class="flex-grow-1">
          <h6 class="mb-1">${item.productName}</h6>
          
          <div class="mt-2 d-flex align-items-center gap-2">
            <button class="btn btn-sm btn-outline-secondary" onclick="updateQuantity(${
              item.id
            }, ${item.quantity - 1})">−</button>
            <span>${item.quantity}</span>
            <button class="btn btn-sm btn-outline-secondary" onclick="updateQuantity(${
              item.id
            }, ${item.quantity + 1})">+</button>
            <button class="btn btn-sm btn-outline-danger ms-3" onclick="removeFromCart(${
              item.id
            })">
              <i class="bi bi-trash"></i> Xoá
            </button>
          </div>
        </div>
        <div class="text-end fw-bold">${itemTotal.toLocaleString()} đ</div>
      </div>
    `;
  }

  const total = subtotal;

  document.getElementById("subtotalText").innerText =
    subtotal.toLocaleString() + " đ";
  document.getElementById("totalText").innerText =
    total.toLocaleString() + " đ";

  updateCartBadgeFromApi();
}

function updateQuantity(cartItemId, quantity) {
  if (quantity <= 0) return removeFromCart(cartItemId);

  fetch(`${API_CART}/items/${cartItemId}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ quantity }),
  })
    .then(() => loadCart())
    .catch(() => alert("Lỗi cập nhật số lượng!"));
}

function removeFromCart(cartItemId) {
  fetch(`${API_CART}/items/${cartItemId}`, {
    method: "DELETE",
    headers: { Authorization: `Bearer ${token}` },
  })
    .then(() => loadCart())
    .catch(() => alert("Lỗi khi xoá sản phẩm!"));
}

async function updateCartBadgeFromApi() {
  const badge = document.getElementById("cartCountBadge");
  if (!badge || !token) return;

  try {
    const res = await fetch(API_CART, {
      headers: { Authorization: `Bearer ${token}` },
    });

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
    console.error("Lỗi cập nhật badge:", err);
  }
}

document.addEventListener("DOMContentLoaded", loadCart);
