document.addEventListener("DOMContentLoaded", function() {
  const ratings = document.querySelectorAll("#rating");

  ratings.forEach(ratingElement => {
    const rating = parseFloat(ratingElement.querySelector(".rating-count").textContent);
    const stars = ratingElement.querySelectorAll(".fa-star");
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating - fullStars >= 0.5;

    for (let i = 0; i < stars.length; i++) {
      if (i < fullStars) {
        stars[i].classList.add("checked");
      } else if (i === fullStars && hasHalfStar) {
        stars[i].classList.add("checked", "fa-star-half-o");
      } else {
        stars[i].classList.add("fa-star-o");
      }
    }
  });
});


// Example usage

function increaseValue(button, limit) {
  const numberInput = button.parentElement.querySelector(".number");
  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (limit && value >= limit) return;
  numberInput.innerHTML = value + 1;
}

function decreaseValue(button) {
  const numberInput = button.parentElement.querySelector(".number");
  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (value < 1) return;
  numberInput.innerHTML = value - 1;
}
