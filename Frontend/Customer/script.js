document.addEventListener("DOMContentLoaded", function () {
  let multipleCardCarousel = document.querySelector("#carouselExampleControls");

  if (window.matchMedia("(min-width: 768px)").matches) {
    let carousel = new bootstrap.Carousel(multipleCardCarousel, {
      interval: false, // Disable automatic sliding
      wrap: false, // Prevent wrapping at the end
    });

    let carouselWidth = document.querySelector(".carousel-inner").scrollWidth;
    let cardWidth = document.querySelector(".carousel-item").offsetWidth;
    let scrollPosition = 0;

    document
      .querySelector("#carouselExampleControls .carousel-control-next")
      .addEventListener("click", function () {
        if (scrollPosition < carouselWidth - cardWidth * 4) {
          scrollPosition += cardWidth;
        } else {
          scrollPosition = 0; 
        }
        document
          .querySelector("#carouselExampleControls .carousel-inner")
          .scroll({ left: scrollPosition, behavior: "smooth" });
      });

    document
      .querySelector("#carouselExampleControls .carousel-control-prev")
      .addEventListener("click", function () {
        if (scrollPosition > 0) {
          scrollPosition -= cardWidth;
        } else {
          scrollPosition = carouselWidth - cardWidth * 4; // Go to the last position
        }
        document
          .querySelector("#carouselExampleControls .carousel-inner")
          .scroll({ left: scrollPosition, behavior: "smooth" });
      });
  } else {
    multipleCardCarousel.classList.add("slide");
  }
});
function setRating(rating) {
    const stars = document.querySelectorAll('#rating .fa-star');
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating - fullStars >= 0.5;

    for (let i = 0; i < stars.length; i++) {
      if (i < fullStars) {
        stars[i].classList.add('checked');
      } else if (i === fullStars && hasHalfStar) {
        stars[i].classList.add('checked', 'fa-star-half-o');
      } 
      else {
        stars[i].classList.add('fa-star-o');
      }
  
    }
  }

  // Example usage
  setRating(4.5); // 
