console.log("✅ header-auth.js loaded");

document.addEventListener("DOMContentLoaded", () => {
  const userRaw = localStorage.getItem("user");
  const user = userRaw ? JSON.parse(userRaw) : null;

  const logoutLink = document.getElementById("logoutLink");
  const userNameDisplay = document.getElementById("userNameDisplay");

  if (userNameDisplay && user) {
    userNameDisplay.innerText = user.fullName || user.email || "Tài khoản";
  }

  if (logoutLink) {
    logoutLink.addEventListener("click", (e) => {
      e.preventDefault();
      localStorage.clear();
      window.location.href = "/index.html";
    });
  }
});
