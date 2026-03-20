const API_REVIEWS = "http://localhost:5287/api/reviews";
const token = localStorage.getItem("token");

async function fetchReviews() {
  const res = await fetch(API_REVIEWS, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });
  return res.ok ? await res.json() : [];
}

async function renderReviews() {
  const reviews = await fetchReviews();
  const table = document.getElementById("reviewTableBody");
  const search = document.getElementById("searchInput").value.toLowerCase();
  const status = document.getElementById("filterStatus").value;
  table.innerHTML = "";

  const filtered = reviews.filter((r) => {
    const nameMatch = r.productName?.toLowerCase().includes(search);
    const statusMatch = status === "" || String(r.isHidden) === status;
    return nameMatch && statusMatch;
  });

  if (filtered.length === 0) {
    table.innerHTML = `<tr><td colspan="8" class="text-center">Không có đánh giá phù hợp</td></tr>`;
    return;
  }

  filtered.forEach((r) => {
    table.innerHTML += `
      <tr>
        <td>${r.id}</td>
        <td>${r.productName || "Không rõ"}</td>
        <td>${r.userName || "Ẩn danh"}</td>
        <td>${renderStars(r.rating)}</td>
        <td>${r.comment}</td>
        <td>${new Date(r.createdAt).toLocaleString()}</td>
        <td>${r.isHidden ? "🚫 Đã ẩn" : "✅ Hiển thị"}</td>
        <td>
          <button class="btn btn-sm btn-warning" onclick="toggleReview(${
            r.id
          }, ${!r.isHidden})">
            ${r.isHidden ? "Hiện" : "Ẩn"}
          </button>
        </td>
      </tr>`;
  });
}

async function toggleReview(id, isHidden) {
  const res = await fetch(`${API_REVIEWS}/${id}/hide?hide=${isHidden}`, {
    method: "PUT",
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ isHidden }),
  });

  if (res.ok) {
    renderReviews();
  } else {
    alert("Không thể cập nhật trạng thái đánh giá.");
  }
}

function renderStars(rating) {
  let html = "";
  for (let i = 1; i <= 5; i++) {
    html += `<i class="bi ${
      i <= rating ? "bi-star-fill" : "bi-star"
    } text-warning"></i>`;
  }
  return html;
}

document.addEventListener("DOMContentLoaded", renderReviews);
