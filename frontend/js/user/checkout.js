const API_CART = "http://localhost:5287/api/carts";
const API_ORDER = "http://localhost:5287/api/orders";
const token = localStorage.getItem("token");

let cartItems = [];

window.addEventListener("DOMContentLoaded", () => {
  loadCart();
  loadCities();
  document.getElementById("city").addEventListener("change", loadDistricts);
  document.getElementById("district").addEventListener("change", loadWards);
});

async function loadCart() {
  try {
    const res = await fetch(API_CART, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();

    if (!data || !data.cartItems || data.cartItems.length === 0) {
      document.getElementById("orderSummary").innerHTML =
        "<div class='alert alert-warning'>Giỏ hàng trống!</div>";
      return;
    }

    cartItems = data.cartItems;
    let html = "<ul class='list-group mb-3'>";
    let total = 0;

    cartItems.forEach((item) => {
      const sub = item.unitPrice * item.quantity;
      total += sub;
      html += `<li class="list-group-item d-flex align-items-center">
        <img src="http://localhost:5287${
          item.productImage || "/uploads/default.jpg"
        }"
             class="me-3 rounded" width="60" height="60" style="object-fit:cover" />
        <div class="flex-grow-1">
          <div><strong>${item.productName}</strong></div>
          <div>SL: ${item.quantity}</div>
        </div>
        <div class="fw-bold">${sub.toLocaleString()} đ</div>
      </li>`;
    });

    html += `</ul><div class="fw-bold text-end">Tổng cộng: ${total.toLocaleString()} đ</div>`;
    document.getElementById("orderSummary").innerHTML = html;
  } catch (err) {
    console.error("Lỗi khi tải giỏ hàng:", err);
  }
}

async function submitOrder() {
  const fullName = document.getElementById("fullName").value.trim();
  const phone = document.getElementById("phone").value.trim();
  const email = document.getElementById("email").value.trim();
  const address = document.getElementById("address").value.trim();
  const city =
    document.getElementById("city").selectedOptions[0]?.textContent || "";
  const district =
    document.getElementById("district").selectedOptions[0]?.textContent || "";
  const ward =
    document.getElementById("ward").selectedOptions[0]?.textContent || "";
  const note = document.getElementById("note").value.trim();
  const paymentMethod =
    document.querySelector("input[name='payment']:checked")?.value || "COD";

  if (
    !fullName ||
    !phone ||
    !email ||
    !address ||
    !city ||
    !district ||
    !ward
  ) {
    alert("Vui lòng nhập đầy đủ thông tin giao hàng.");
    return;
  }

  const shippingAddress = `${address}, ${ward}, ${district}, ${city}`;
  const fullNote = `[HT: ${paymentMethod}] ${note}`;

  if (!cartItems.length) {
    alert("Không thể đặt hàng vì giỏ hàng trống.");
    return;
  }

  const orderItems = cartItems.map((item) => ({
    productId: item.productId,
    quantity: item.quantity,
    unitPrice: item.unitPrice,
  }));

  const orderData = {
    customerName: fullName,
    phone,
    email,
    shippingAddress,
    note: fullNote,
    orderItems: orderItems,
  };

  console.log("🚚 Gửi đơn hàng với địa chỉ:", shippingAddress);
  console.log("🧾 Dữ liệu đơn hàng:", orderData);

  try {
    const res = await fetch("http://localhost:5287/api/orders", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(orderData),
    });

    if (res.ok) {
      alert("Đặt hàng thành công!");
      await fetch("http://localhost:5287/api/carts/clear", {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      window.location.href = "/pages/user/order.html";
    } else {
      const result = await res.json();
      console.error("❌ Đặt hàng lỗi:", result);
      alert("Đặt hàng thất bại: " + (result.message || "Lỗi không xác định"));
    }
  } catch (err) {
    alert("Lỗi kết nối khi đặt hàng: " + err.message);
  }
}

async function loadCities() {
  const res = await fetch("https://provinces.open-api.vn/api/p/");
  const cities = await res.json();
  const citySelect = document.getElementById("city");
  cities.forEach((city) => {
    const option = document.createElement("option");
    option.value = city.code;
    option.textContent = city.name;
    citySelect.appendChild(option);
  });
}

async function loadDistricts() {
  const cityCode = document.getElementById("city").value;
  const districtSelect = document.getElementById("district");
  const wardSelect = document.getElementById("ward");
  districtSelect.innerHTML = '<option value="">Chọn Quận/Huyện *</option>';
  wardSelect.innerHTML = '<option value="">Chọn Phường/Xã *</option>';
  if (!cityCode) return;

  const res = await fetch(
    `https://provinces.open-api.vn/api/p/${cityCode}?depth=2`
  );
  const data = await res.json();
  data.districts.forEach((d) => {
    const option = document.createElement("option");
    option.value = d.code;
    option.textContent = d.name;
    districtSelect.appendChild(option);
  });
}

async function loadWards() {
  const districtCode = document.getElementById("district").value;
  const wardSelect = document.getElementById("ward");
  wardSelect.innerHTML = '<option value="">Chọn Phường/Xã *</option>';
  if (!districtCode) return;

  const res = await fetch(
    `https://provinces.open-api.vn/api/d/${districtCode}?depth=2`
  );
  const data = await res.json();
  data.wards.forEach((w) => {
    const option = document.createElement("option");
    option.value = w.name;
    option.textContent = w.name;
    wardSelect.appendChild(option);
  });
}
