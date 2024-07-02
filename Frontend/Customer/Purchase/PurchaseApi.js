import { GetData, PostData } from "../Api/Api.js";

var Product = [];

function getQueryParam(param) {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get(param);
}

document.onload = await InitializeData();

async function InitializeData() {
  const searchElement = getQueryParam("search");
  if (searchElement) {
    Product = await GetData("http://localhost:5033/api/View/ViewAllItems", {
      searchContent: searchElement,
    });
    FilterData();
  }
}

function CreateElement(FilterResult, currentPage = 1, itemsPerPage = 10) {
  var container = document.getElementById("item-cont");
  var content = "";

  // Calculate start and end index for current page
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const paginatedResult = FilterResult.slice(startIndex, endIndex);

  paginatedResult.forEach((data, i) => {
    content += `
      <div class="card dark">
        <div class="img-cont">
          <img
            src="data:image/jpeg;base64,${data.image}"
            class="card-img-top"
            alt="..."
          />
        </div>
        <div class="card-body">
          <div class="text-section">
            <h5 class="card-title fw-bold">
              ${data.medicineName}
            </h5>
            <p class="card-text company">${data.brandName}</p>
            <div id="rating">
              <span class="rating-count">${i}</span>
              <span class="fa fa-star" id="1"></span>
              <span class="fa fa-star" id="2"></span>
              <span class="fa fa-star" id="3"></span>
              <span class="fa fa-star" id="4"></span>
              <span class="fa fa-star" id="5"></span>
              <span>(10)</span>
            </div>
            <div class="card-cont">
              <p class="card-text">Weight:${data.weight}</p>
              <p class="card-text">Pack of ${data.itemPerPack} Item${data.ItemPerPack>1? "S":""}</p>
            </div>
          </div>
          <div class="cta-section">
            <div class="price">$${data.amount}</div>
            <div class="quantity-field">
              <a href="../Product/Product.html?productId=${data.medicineId}">
                <button class="product-btn">View Product</button>
              </a>
            </div>
          </div>
        </div>
      </div>`;
  });

  // Add pagination buttons
  content += createPaginationButtons(
    FilterResult.length,
    itemsPerPage,
    currentPage
  );

  container.innerHTML = content;
  SetRating();

  // Add event listeners to pagination buttons
  const pageButtons = document.querySelectorAll(".page-btn");
  pageButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const pageNum = parseInt(button.getAttribute("data-page"));
      CreateElement(FilterResult, pageNum, itemsPerPage);
    });
  });
}

function createPaginationButtons(totalItems, itemsPerPage, currentPage) {
  const totalPages = Math.ceil(totalItems / itemsPerPage);
  let paginationHtml = '<div class="pagination">';
  for (let i = 1; i <= totalPages; i++) {
    paginationHtml += `<button class="page-btn ${
      i === currentPage ? "active" : ""
    }" data-page="${i}">${i}</button>`;
  }
  paginationHtml += "</div>";
  return paginationHtml;
}

document.getElementById("filter-btn").addEventListener("click", FilterData);

function FilterData() {
  var select = document.getElementById("filter-select").value;
  var min = Number(document.getElementById("min").value);
  var max = Number(document.getElementById("max").value);

  var FilterProduct = Product.filter(
    (product) => product.amount >= min && product.amount <= max
  );

  if (select == "1") {
    FilterProduct.sort((a, b) => a.amount - b.amount);
  } else if (select == "2") {
    FilterProduct.sort((a, b) => b.amount - a.amount);
  }

  CreateElement(FilterProduct, 1, 5);
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
