const API_PRODUCTS = "http://localhost:5287/api/Products";
const API_REVIEWS = "http://localhost:5287/api/reviews";
const token = localStorage.getItem("token");

const urlParams = new URLSearchParams(window.location.search);
const productId = urlParams.get("productId");

if (!productId) {
  alert("Không tìm thấy sản phẩm.");
}

window.productId = productId;
async function loadProduct() {
  try {
    const res = await fetch(`${API_PRODUCTS}/${productId}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();

    if (!res.ok) throw new Error(data.message || "Không thể tải sản phẩm");

    document.getElementById("productName").innerText = data.name;
    document.getElementById(
      "productPrice"
    ).innerText = `Giá: ${data.price.toLocaleString()}đ`;
    document.getElementById("productDescription").innerText = `Mô tả: ${
      data.description || "Không có"
    }`;
    document.getElementById(
      "productImage"
    ).src = `http://localhost:5287${data.imageUrl}`;
  } catch (err) {
    console.error("Lỗi tải sản phẩm:", err);
    document.getElementById("productName").innerText = "Lỗi khi tải sản phẩm";
  }
}

async function loadReviews() {
  try {
    const res = await fetch(`${API_REVIEWS}/product/${productId}`);
    const reviews = await res.json();

    const container = document.getElementById("reviewList");
    const countSpan = document.getElementById("reviewCount");
    container.innerHTML = "";

    if (Array.isArray(reviews)) {
      countSpan.innerText = reviews.length;
      if (reviews.length === 0) {
        container.innerHTML = "<p>Chưa có đánh giá nào.</p>";
        return;
      }

      reviews.forEach((r) => {
        container.innerHTML += `
          <div class="border-bottom pb-3 mb-3">
            <div class="d-flex align-items-center mb-1">
              ${renderStars(r.rating)}
              <strong class="ms-2">${r.user?.fullName || "Ẩn danh"}</strong>
            </div>
            <p class="mb-1">${r.comment}</p>
            <small class="text-muted">${new Date(
              r.createdAt
            ).toLocaleString()}</small>
          </div>
        `;
      });
    } else {
      throw new Error("reviews is not an array");
    }
  } catch (err) {
    console.error("Lỗi tải đánh giá:", err);
    document.getElementById("reviewList").innerText = "Lỗi tải đánh giá";
  }
}

function renderStars(rating) {
  let stars = "";
  for (let i = 1; i <= 5; i++) {
    stars += `<i class="fas fa-star ${
      i <= rating ? "filled" : ""
    }" style="color: ${i <= rating ? "#ff6f91" : "#ccc"}"></i>`;
  }
  return stars;
}

function initStarRating() {
  const stars = document.querySelectorAll("#starRating .fas");
  const ratingInput = document.getElementById("rating");

  stars.forEach((star) => {
    star.addEventListener("click", () => {
      const value = parseInt(star.dataset.value);
      ratingInput.value = value;

      stars.forEach((s, index) => {
        if (index < value) {
          s.classList.add("filled");
          s.style.color = "#ff6f91";
        } else {
          s.classList.remove("filled");
          s.style.color = "#ccc";
        }
      });
    });

    star.addEventListener("mouseover", () => {
      const value = parseInt(star.dataset.value);
      stars.forEach((s, index) => {
        if (index < value) {
          s.style.color = "#ff6f91";
        } else {
          s.style.color = "#ccc";
        }
      });
    });

    star.addEventListener("mouseout", () => {
      const currentRating = parseInt(ratingInput.value);
      stars.forEach((s, index) => {
        if (index < currentRating) {
          s.style.color = "#ff6f91";
          s.classList.add("filled");
        } else {
          s.style.color = "#ccc";
          s.classList.remove("filled");
        }
      });
    });
  });
}

document.getElementById("reviewForm")?.addEventListener("submit", async (e) => {
  e.preventDefault();
  const rating = parseInt(document.getElementById("rating").value);
  const comment = document.getElementById("comment").value;

  if (!token) {
    alert("Bạn cần đăng nhập để đánh giá!");
    return;
  }

  try {
    const res = await fetch(API_REVIEWS, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify({ productId, rating, comment }),
    });

    const result = await res.json();
    if (res.ok) {
      alert("Đánh giá thành công!");
      document.getElementById("reviewForm").reset();
      loadReviews();
    } else {
      alert(result.message || "Lỗi khi gửi đánh giá");
    }
  } catch (err) {
    alert("Lỗi gửi đánh giá: " + err.message);
  }
});

document.addEventListener("DOMContentLoaded", () => {
  loadProduct();
  loadReviews();
  initStarRating();
});
