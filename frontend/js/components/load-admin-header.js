document.addEventListener("DOMContentLoaded", () => {
  fetch("/components/header-admin.html")
    .then((res) => res.text())
    .then((html) => {
      const container = document.getElementById("header-container");
      if (container) {
        container.innerHTML = html;

        const logoutBtn = document.getElementById("logoutBtn");
        if (logoutBtn) {
          logoutBtn.addEventListener("click", (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.href = "/pages/index.html";
          });
        }
      }
    })
    .catch((err) => console.error("Lỗi khi load header admin:", err));
});
