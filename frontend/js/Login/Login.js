document.addEventListener("DOMContentLoaded", function () {
  const loginForm = document.getElementById("loginForm");
  if (loginForm) {
    loginForm.addEventListener("submit", async (e) => {
      e.preventDefault();

      const email = document.getElementById("email").value;
      const password = document.getElementById("password").value;

      try {
        const response = await fetch("http://localhost:5287/api/Auth/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            email,
            password,
          }),
        });

        const data = await response.json();

        if (response.ok) {
          const user = {
            token: data.token,
            roles: data.roles,
            email: data.email,
            fullName: data.fullName,
          };

          localStorage.setItem("user", JSON.stringify(user));
          localStorage.setItem("token", data.token);
          localStorage.setItem("userId", data.id);

          alert("Đăng nhập thành công!");

          if (Array.isArray(data.roles) && data.roles.includes("Admin")) {
            window.location.href = "admin/dashboard/admin-dashboard.html";
          } else {
            window.location.href = "user/home.html";
          }
        } else {
          alert(data.message || "Đăng nhập thất bại");
        }
      } catch (error) {
        console.error("Lỗi khi gọi API:", error);
        alert("Đã xảy ra lỗi khi đăng nhập");
      }
    });
  }

  const registerForm = document.getElementById("registerForm");
  if (registerForm) {
    registerForm.addEventListener("submit", async (e) => {
      e.preventDefault();

      const fullName = document.getElementById("fullname").value;
      const email = document.getElementById("reg-email").value;
      const password = document.getElementById("reg-password").value;
      const confirmPassword = document.getElementById("confirmPassword").value;
      const address = "";
      if (password !== confirmPassword) {
        alert("Mật khẩu xác nhận không khớp!");
        return;
      }

      try {
        const response = await fetch(
          "http://localhost:5287/api/Auth/register",
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              fullName,
              email,
              password,
              confirmPassword,
              address,
            }),
          }
        );

        const data = await response.json();

        if (response.ok) {
          alert("Đăng ký thành công! Vui lòng đăng nhập.");
          document
            .getElementById("container")
            .classList.remove("right-panel-active");
        } else {
          alert(data.message || "Đăng ký thất bại");
        }
      } catch (error) {
        console.error("Lỗi khi gọi API:", error);
        alert("Đã xảy ra lỗi khi đăng ký");
      }
    });
  }

  const registerBtn = document.getElementById("register");
  const loginBtn = document.getElementById("login");
  const container = document.getElementById("container");

  if (registerBtn && loginBtn && container) {
    registerBtn.addEventListener("click", () => {
      container.classList.add("right-panel-active");
    });

    loginBtn.addEventListener("click", () => {
      container.classList.remove("right-panel-active");
    });
  }
});
