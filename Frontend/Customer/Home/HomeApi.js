import { GetData, PostData } from "../Api/Api.js";
var token = localStorage.getItem("token");
document.onload = Validate();
async function Validate() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    var validate = await GetData(
      "http://localhost:5033/api/Auth/validate",
      {},
      token
    );
    console.log(validate);
    if (validate.role != "User") {
      document.getElementById("profile").style.display = "none";
      document.getElementById("cart-icon").style.display = "none";
      document.querySelector(".login-btn").style.display = "block";
    }
  } catch (error) {
    document.getElementById("profile").style.display = "none";
    document.getElementById("cart-icon").style.display = "none";
    document.querySelector(".login-btn").style.display = "block";
    console.log(error);
  } finally {
    await FetchData();
  }
}

async function FetchData() {
  try {
    var BestSeller = await GetData(
      "http://localhost:5033/api/View/GetBestSeller",
      {},
      null
    );
    console.log(BestSeller);
    CreateBestSeller(BestSeller);
    var BestCategory = await GetData(
      "http://localhost:5033/api/View/GetBestCategory",
      {},
      null
    );
    CreateCategory(BestCategory);
    console.log(BestCategory);
  } catch (error) {
    console.log(error);
  } finally {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "hidden";
  }
}
function CreateBestSeller(products) {
  const slider = document.querySelector(".slider");
  const prevBtn = document.querySelector(".prev");
  const nextBtn = document.querySelector(".next");
  let currentIndex = 0;

  function getProductsPerSlide() {
    if (window.innerWidth > 1024) return 4;
    if (window.innerWidth > 768) return 3;
    if (window.innerWidth > 480) return 2;
    return 1;
  }

  function createProductElement(product) {
    return `
         <div class="product">
    <div class="best-img">
        <img src="data:image/jpeg;base64,${product.image}">
    </div>
    <div class="card-text-cont">
        <h5 class="card-title">${product.medicineName}</h5>
        <p class="card-text">${product.brand}</p>
        <div id="rating">
            <span class="rating-count">${product.rating.toFixed(1)}</span>
            <span class="fa fa-star" id="1"></span>
            <span class="fa fa-star" id="2"></span>
            <span class="fa fa-star" id="3"></span>
            <span class="fa fa-star" id="4"></span>
            <span class="fa fa-star" id="5"></span>
            <span>(${product.feedbackCount})</span>
        </div>
        <div class="product-foot">
            <p class="card-rate">$${product.price}</p>
            <a href="../Product/Product.html?productId=${
              product.medicineId
            }" class="btn btn-primary add-cart">View item</a>
        </div>
    </div>
</div>
      `;
  }

  document.getElementById("logout").addEventListener("click", () => {
    localStorage.removeItem("token");
    location.reload();
  });
  function updateSlider() {
    slider.innerHTML = "";
    const productsPerSlide = getProductsPerSlide();
    for (let i = 0; i < products.length; i += productsPerSlide) {
      const slide = document.createElement("div");
      slide.className = "slide";
      slide.innerHTML = products
        .slice(i, i + productsPerSlide)
        .map(createProductElement)
        .join("");
      slider.appendChild(slide);
    }
    showSlide(0);
    SetRating();
  }

  function showSlide(index) {
    const slides = document.querySelectorAll(".slide");
    if (index < 0) {
      currentIndex = slides.length - 1;
    } else if (index >= slides.length) {
      currentIndex = 0;
    } else {
      currentIndex = index;
    }

    const offset = -currentIndex * 100;
    slider.style.transform = `translateX(${offset}%)`;
  }

  prevBtn.addEventListener("click", (e) => {
    e.preventDefault();
    showSlide(currentIndex - 1);
  });

  nextBtn.addEventListener("click", (e) => {
    e.preventDefault();
    showSlide(currentIndex + 1);
  });

  window.addEventListener("resize", updateSlider);
  updateSlider();
}
function CreateCategory(data) {
  var container = document.getElementById("category-cont");
  var content = "";
  data.forEach((element) => {
    content += `<div class="col-lg-4 col-md-6 col-sm-12 d-flex justify-content-center">
             <a href="../Purchase/Purchase.html?category=${element.categoryName}"><div class="box"> <div class="cat-img-cont"><img class="cat-img"  src="data:image/jpeg;base64,${element.image}" alt=""></div><div class="cat-cont"><p>${element.categoryName}</p></div></div> </a>
          </div>
              `;
  });
  container.innerHTML = content;
}
document.getElementById("search-btn").addEventListener("click", Search);

function Search() {
  var search = document.getElementById("search-value").value;
  console.log(`../Purchase/Purchase.html/?search=${search}`);
  window.location.href = `../Purchase/Purchase.html?search=${search}`;
}
function SetRating() {
  const ratings = document.querySelectorAll("#rating");
  ratings.forEach((ratingElement) => {
    const rating = parseFloat(
      ratingElement.querySelector(".rating-count").textContent
    );
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
}
