document.addEventListener("DOMContentLoaded", () => {
  fetch("../../components/header.html")
    .then((res) => res.text())
    .then((data) => {
      document.getElementById("header-container").innerHTML = data;

      const storedUser = localStorage.getItem("user");
      const user = storedUser ? JSON.parse(storedUser) : null;
      const token = localStorage.getItem("token");

      const loginLink = document.getElementById("loginLink");
      const logoutBtn = document.getElementById("logoutBtn");
      const userNameDisplay = document.getElementById("userNameDisplay");

      if (user && user.fullName && token) {
        userNameDisplay.textContent = `👤 ${user.fullName}`;
        userNameDisplay.classList.remove("d-none");
        loginLink?.classList.add("d-none");
        logoutBtn?.classList.remove("d-none");

        updateCartBadgeFromApi();
      } else {
        loginLink?.classList.remove("d-none");
        logoutBtn?.classList.add("d-none");
        userNameDisplay?.classList.add("d-none");
      }

      logoutBtn?.addEventListener("click", () => {
        localStorage.clear();
        alert("Đã đăng xuất!");
        location.href = "../../pages/login.html";
      });

      async function updateCartBadgeFromApi() {
        const badge = document.getElementById("cartCountBadge");
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
          console.error("Lỗi khi lấy giỏ hàng:", err);
        }
      }
    });

  fetch("../../components/footer.html")
    .then((res) => res.text())
    .then((data) => {
      document.getElementById("footer-container").innerHTML = data;
    });
});
