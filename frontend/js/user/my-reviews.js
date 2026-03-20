const API_REVIEWS = "http://localhost:5287/api/reviews/my";
const token = localStorage.getItem("token");

async function loadReviews() {
  const container = document.getElementById("reviewList");
  container.innerHTML = "";

  const res = await fetch(API_REVIEWS, {
    headers: { Authorization: `Bearer ${token}` },
  });
  const reviews = await res.json();

  if (!reviews.length) {
    container.innerHTML = `<div class="alert alert-info">Bạn chưa đánh giá sản phẩm nào.</div>`;
    return;
  }

  reviews.forEach((r) => {
    const item = document.createElement("div");
    item.className = "card mb-3";
    item.innerHTML = `
      <div class="card-body">
        <h5 class="card-title">🛍️ ${r.productName}</h5>
        <p><strong>⭐ Số sao:</strong> <span id="stars-${r.id}">${r.rating}</span></p>
        <p><strong>📝 Nội dung:</strong> <span id="content-${r.id}">${r.comment}</span></p>

        <button class="btn btn-sm btn-outline-primary me-2" onclick="editReview(${r.id})">✏️ Sửa</button>
        <button class="btn btn-sm btn-outline-danger" onclick="deleteReview(${r.id})">🗑️ Xoá</button>

        <div id="edit-form-${r.id}" class="mt-3 d-none">
          <input id="edit-rating-${r.id}" class="form-control mb-2" type="number" min="1" max="5" value="${r.rating}">
          <textarea id="edit-content-${r.id}" class="form-control mb-2">${r.comment}</textarea>
          <button class="btn btn-success btn-sm" onclick="saveReview(${r.id})">💾 Lưu</button>
          <button class="btn btn-secondary btn-sm" onclick="cancelEdit(${r.id})">Huỷ</button>
        </div>
      </div>
    `;
    container.appendChild(item);
  });
}

function editReview(id) {
  document.getElementById(`edit-form-${id}`).classList.remove("d-none");
}

function cancelEdit(id) {
  document.getElementById(`edit-form-${id}`).classList.add("d-none");
}

async function saveReview(id) {
  const rating = +document.getElementById(`edit-rating-${id}`).value;
  const content = document.getElementById(`edit-content-${id}`).value;

  const resReview = await fetch(`${API_REVIEWS}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  const allReviews = await resReview.json();
  const target = allReviews.find((r) => r.id === id);
  const productId = target?.productId;

  if (!productId) {
    alert("Không tìm thấy sản phẩm để cập nhật đánh giá.");
    return;
  }

  const res = await fetch(`http://localhost:5287/api/reviews/${id}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      productId,
      rating,
      comment: content,
    }),
  });

  if (res.ok) {
    await loadReviews();
  } else {
    const err = await res.json();
    alert(
      " Lỗi khi cập nhật đánh giá:\n" +
        (err?.errors?.join("\n") || "Lỗi không xác định")
    );
  }
}

document.addEventListener("DOMContentLoaded", loadReviews);
