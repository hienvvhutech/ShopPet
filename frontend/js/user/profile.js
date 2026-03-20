const API_URL = "http://localhost:5287/api/users/profile";
const token = localStorage.getItem("token");

if (!token) {
  alert("Vui lòng đăng nhập.");
  window.location.href = "/login.html";
}

document.addEventListener("DOMContentLoaded", async () => {
  await loadProvinces();

  const res = await fetch(API_URL, {
    headers: { Authorization: `Bearer ${token}` },
  });

  const user = await res.json();
  document.getElementById("fullName").value = user.fullName || "";
  document.getElementById("email").value = user.email || "";
  document.getElementById("createdAt").value = new Date(
    user.createdAt
  ).toLocaleDateString();

  document.getElementById("phone").value = user.phoneNumber || "";

  // Tách địa chỉ nếu có sẵn (Tỉnh|Huyện|Xã)
  if (user.address) {
    const parts = user.address.split("|");
    if (parts.length === 3) {
      document.getElementById("province").value = parts[0];
      await loadDistricts(parts[0]);
      document.getElementById("district").value = parts[1];
      await loadWards(parts[1]);
      document.getElementById("ward").value = parts[2];
    }
  }
});

document.getElementById("province").addEventListener("change", async (e) => {
  await loadDistricts(e.target.value);
  document.getElementById("ward").innerHTML = "";
});

document.getElementById("district").addEventListener("change", async (e) => {
  await loadWards(e.target.value);
});

async function loadProvinces() {
  const res = await fetch("https://provinces.open-api.vn/api/p/");
  const data = await res.json();
  const select = document.getElementById("province");
  select.innerHTML = data
    .map((p) => `<option value="${p.name}">${p.name}</option>`)
    .join("");
}

async function loadDistricts(provinceName) {
  const res = await fetch(`https://provinces.open-api.vn/api/p`);
  const provinces = await res.json();
  const province = provinces.find((p) => p.name === provinceName);
  if (!province) return;

  const res2 = await fetch(
    `https://provinces.open-api.vn/api/p/${province.code}?depth=2`
  );
  const data = await res2.json();
  const select = document.getElementById("district");
  select.innerHTML = data.districts
    .map((d) => `<option value="${d.name}">${d.name}</option>`)
    .join("");
}

async function loadWards(districtName) {
  const res = await fetch(`https://provinces.open-api.vn/api/d`);
  const districts = await res.json();
  const district = districts.find((d) => d.name === districtName);
  if (!district) return;

  const res2 = await fetch(
    `https://provinces.open-api.vn/api/d/${district.code}?depth=2`
  );
  const data = await res2.json();
  const select = document.getElementById("ward");
  select.innerHTML = data.wards
    .map((w) => `<option value="${w.name}">${w.name}</option>`)
    .join("");
}

document.getElementById("profileForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const address = [
    document.getElementById("province").value,
    document.getElementById("district").value,
    document.getElementById("ward").value,
  ].join("|");

  const body = {
    fullName: document.getElementById("fullName").value,
    address,
    phoneNumber: document.getElementById("phone").value,
  };

  const res = await fetch(API_URL, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(body),
  });

  const result = await res.json();
  const msgDiv = document.getElementById("updateMsg");
  msgDiv.textContent = res.ok
    ? "✅ " + result.message
    : "❌ " + (result.message || "Thất bại");
  msgDiv.className = res.ok ? "text-success fw-bold" : "text-danger fw-bold";
});
