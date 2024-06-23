document.addEventListener("DOMContentLoaded", function () {
  const toggler = document.getElementById("toggler");
  const spans = document.querySelectorAll(".sidebar-link span");

  toggler.addEventListener("change", function () {
    if (this.checked) {
      spans.forEach((span) => {
        span.style.display = "inline";
      });
    } else {
      setTimeout(() => {
        spans.forEach((span) => {
          span.style.display = "none";
        });
      }, 350); // Delay matches the transition duration
    }
  });
});
