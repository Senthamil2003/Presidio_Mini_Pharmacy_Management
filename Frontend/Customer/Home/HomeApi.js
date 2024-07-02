import { GetData, PostData } from "../Api/Api.js";

document.onload = FetchData();
async function FetchData() {
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
}
function CreateBestSeller(data) {
  var container = document.getElementById("card-cont");
  var content = "";
  data.forEach((element) => {
    content += `<div class="carousel-item">
                <div class="card custom-card">
                  <div class="img-wrapper">
                    <img
                     src="data:image/jpeg;base64,${element.image}"
                      class="d-block w-100"
                      alt="..."
                    />
                  </div>
                  <div class="card-body">
                    <h5 class="card-title">${element.medicineName}</h5>
                    <p class="card-text">${element.brand}</p>

                    <div id="rating">
                      <span>${element.rating}</span>
                      <span class="fa fa-star" id="1"></span>
                      <span class="fa fa-star" id="2"></span>
                      <span class="fa fa-star" id="3"></span>
                      <span class="fa fa-star" id="4"></span>
                      <span class="fa fa-star" id="5"></span>
                      <span>(10)</span>
                    </div>

                    <p class="card-rate"> $${element.price}</p>

                    <a href="#" class="btn btn-primary add-cart">Add to cart</a>
                  </div>
                </div>
              </div>
              `;
  });
  container.innerHTML = content;
}
function CreateCategory(data) {
  var container = document.getElementById("category-cont");
  var content = "";
  data.forEach((element) => {
    content += `<div class="col-lg-4 col-md-6 col-sm-12 d-flex justify-content-center">
             <div class="box"><img class="cat-img"  src="data:image/jpeg;base64,${element.image}" alt=""><div class="cat-cont"><p>${element.categoryName}</p></div></div> 
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
