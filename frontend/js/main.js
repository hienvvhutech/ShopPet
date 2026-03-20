document.addEventListener("DOMContentLoaded", () => {
  const header = document.getElementById("header-container");
  const footer = document.getElementById("footer-container");

  if (header) {
    fetch("../components/header.html").then(
      (data) => (header.innerHTML = data)
    );
  }

  if (footer) {
    fetch("../components/footer.html")
      .then((res) => res.text())
      .then((data) => (footer.innerHTML = data));
  }
});
