import { GetData, PostData } from "../Api/Api.js";

var token = localStorage.getItem("token");
var MedicineId;
function getQueryParam(param) {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get(param);
}
var isfeedback = false;
var isCart = false;

document.onload =  Validate();

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
    await InitializeData();
  } catch (error) {
    var spinner = document.querySelector(".custom-spinner");

    document.getElementById("profile").style.display = "none";
    document.getElementById("cart-icon").style.display = "none";
    document.getElementById("dropdownMenuButton1").disabled = true;
    document.getElementById("give-feedback-btn").disabled = true;
    document.getElementById("add-cart").disabled = true;
    
    document.querySelector(".login-btn").style.display = "block";

    console.log(error);
  } finally {
    await InitializeData();
  }
}
async function InitializeData() {
  var spinner = document.querySelector(".custom-spinner");
  spinner.style.visibility = "visible";
  MedicineId = getQueryParam("productId");
  if (token) {
    await MyMedication();

    try {
      var MyFeedback = await GetData(
        "http://localhost:5033/api/Feedback/ViewMyFeedback",
        {},
        token
      );
      MyFeedback.forEach((element) => {
        if (element.medicineId == MedicineId) {
          isfeedback = true;
          return;
        }
      });
    } catch (error) {
      console.log(error);
    }
    try {
      var MyCart = await GetData(
        "http://localhost:5033/api/View/GetOnlyCart",
        {},
        token
      );
      MyCart.forEach((element) => {
        if (element.medicineId == MedicineId) {
          isCart = true;
        }
      });
    } catch (error) {
      console.log(error);
    }

    console.log(isCart, isfeedback);
  }

  if (MedicineId) {
    try {
      var Product = await GetData(
        "http://localhost:5033/api/View/GetMedicine",
        {
          MedicineId: MedicineId,
          token,
        }
      );

      console.log(Product);
      CreateProduct(Product);
    } catch (error) {
      console.log(error);
    }
    try {
      var Feedback = await GetData(
        "http://localhost:5033/api/Feedback/ViewMedicineFeedback",
        {
          medicineId: MedicineId,
          token,
        }
      );
      await CreateFeedback(Feedback);
    } catch (error) {
      var feedbackcont = document.getElementById("feedback-total");
      var starCount = document.getElementById("feedback-rating-count");
      var star = document.getElementById("feedback-count");
      star.innerHTML = 0;
      starCount.innerHTML = 0;
      feedbackcont.innerHTML = `<div class="not-found">No Feedback found</div>`;
      var oneStar = document.getElementById("1-star");
      var twoStar = document.getElementById("2-star");
      var threeStar = document.getElementById("3-star");
      var fourStar = document.getElementById("4-star");
      var fiveStar = document.getElementById("5-star");

      oneStar.style = `width:0%`;
      twoStar.style = `width:0%`;
      threeStar.style = `width:0%`;
      fourStar.style = `width:0%`;
      fiveStar.style = `width:0%`;
    }
    await CreatStar();
  }
  spinner.style.visibility = "hidden";
}

async function CreateProduct(data) {
  var price = document.getElementById("price");
  var star = document.getElementById("rating-star");
  var starCount = document.getElementById("rating-count");
  var medicineName = document.getElementById("medicine-name");
  var company = document.getElementById("company");
  var itemCount = document.getElementById("item-count");
  var description = document.getElementById("description");
  var medicineImage = document.getElementById("medicine-image");
  var viewcart = document.querySelector(".view-cart");
  var quantityField = document.querySelector(".quantity-field");
  var Addtocart = document.getElementById("add-cart");
  if (isCart) {
    viewcart.style.display = "block";
    quantityField.style.display = "none";
    Addtocart.style.display = "none";
  }

  document.getElementById("logout").addEventListener("click", () => {
    localStorage.removeItem("token");
    location.reload();
  });
  medicineImage.src = `data:image/jpeg;base64,${data.imageBase64}`;
  medicineName.innerText = data.medicineName;
  company.innerText = data.brand;
  price.innerText = `$${data.sellingPrice}`;
  star.innerText =
    data.feedbackCount != 0
      ? (Number(data.feedbackSum) / Number(data.feedbackCount)).toFixed(1)
      : 0;
  starCount.innerText = `(${data.feedbackCount})`;
  description.innerText = data.description;
  itemCount.innerText = `Pack of ${data.itemPerPack} item of ${data.weight}`;
}
function formatDate(dateString) {
  var date = new Date(dateString);
  var day = date.getDate().toString().padStart(2, "0");
  var month = (date.getMonth() + 1).toString().padStart(2, "0"); // Months are zero-based
  var year = date.getFullYear();
  return `${month}/${day}/${year}`;
}

function CreateFeedback(data) {
  var starCount = document.getElementById("feedback-rating-count");
  var star = document.getElementById("feedback-count");
  var oneStar = document.getElementById("1-star");
  var twoStar = document.getElementById("2-star");
  var threeStar = document.getElementById("3-star");
  var fourStar = document.getElementById("4-star");
  var fiveStar = document.getElementById("5-star");
  var giveFeedbackButton = document.getElementById("give-feedback-btn");
  if (isfeedback) {
    giveFeedbackButton.innerHTML = "feedback already given";
    giveFeedbackButton.disabled = true;
  }
  star.innerHTML = Number(data.feedbackRating).toFixed(1);
  starCount.innerHTML = data.feedbacks.length;
  oneStar.innerHTML = `${Number(data.ratingPercentages[1]).toFixed(1)}%`;
  twoStar.innerHTML = `${Number(data.ratingPercentages[2]).toFixed(1)}%`;
  threeStar.innerHTML = `${Number(data.ratingPercentages[3]).toFixed(1)}%`;
  fourStar.innerHTML = `${Number(data.ratingPercentages[4]).toFixed(1)}%`;
  fiveStar.innerHTML = `${Number(data.ratingPercentages[5]).toFixed(1)}%`;
  oneStar.style = `width:${Number(data.ratingPercentages[1]).toFixed(1)}%`;
  twoStar.style = `width:${Number(data.ratingPercentages[2]).toFixed(1)}%`;
  threeStar.style = `width:${Number(data.ratingPercentages[3]).toFixed(1)}%`;
  fourStar.style = `width:${Number(data.ratingPercentages[4]).toFixed(1)}%`;
  fiveStar.style = `width:${Number(data.ratingPercentages[5]).toFixed(1)}%`;
  var feedbackcont = document.getElementById("feedback-total");
  var content = "";

  data.feedbacks.forEach((element) => {
    content += `
        <div class="feedback-cont">
              <div class="feedback-head-cont">
                <div class="feedback-img">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    width="30"
                    height="30"
                    fill="currentColor"
                    class="bi bi-person"
                    viewBox="0 0 16 16"
                  >
                    <path
                      d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6m2-3a2 2 0 1 1-4 0 2 2 0 0 1 4 0m4 8c0 1-1 1-1 1H3s-1 0-1-1 1-4 6-4 6 3 6 4m-1-.004c-.001-.246-.154-.986-.832-1.664C11.516 10.68 10.289 10 8 10s-3.516.68-4.168 1.332c-.678.678-.83 1.418-.832 1.664z"
                    />
                  </svg>
                  <div class="d-flex flex-column" style="width: 100%">
                    <p>${element.customerName}</p>
                    <div id="rating">
                      <span class="rating-count">${element.rating}</span>
                      <span class="fa fa-star" id="1"></span>
                      <span class="fa fa-star" id="2"></span>
                      <span class="fa fa-star" id="3"></span>
                      <span class="fa fa-star" id="4"></span>
                      <span class="fa fa-star" id="5"></span>
                      
                    </div>
                  </div>
                </div>
                <div>
                  <p>${formatDate(element.date)}</p>
                </div>
              </div>
              <hr />
              <div>
                <h6>${element.feedbackTitle}</h6>
                <p>
                 ${element.feedbackMessage}
                </p>
              </div>
            </div>
    `;
  });
  if (data.feedbacks.length == 0) {
    content = "No Element Found";
  }
  feedbackcont.innerHTML = content;
}

async function CreatStar() {
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

async function MyMedication() {
  try {
    var Medication = await GetData(
      "http://localhost:5033/api/View/ViewMyMedications",
      {},
      token
    );
    console.log(Medication);
    CreateMedicationList(Medication);
  } catch (error) {
    console.log(error);
  }
}

function CreateMedicationList(data) {
  var listcont = document.getElementById("medication-list");
  var content = `<li id="add-medication">
                    <a class="dropdown-item" data-bs-toggle="modal" data-bs-target="#addmedication" href="#" value="add">+Add Medication</a>
                  </li>`;
  data.forEach((element) => {
    var flag = false;
    element.medicationItems.forEach((item) => {
      if (item.medicineId == MedicineId) {
        flag = true;
        return;
      }
    });
    if (flag) {
      content += ` <li><span class="dropdown-item disabled">${element.medicationName}</span></li>`;
    } else {
      content += ` <li><a class="dropdown-item" value="${element.medicationId}">${element.medicationName}</a></li>`;
    }
  });
  listcont.innerHTML = content;
}

document
  .getElementById("medication-list")
  .addEventListener("click", async function (event) {
    var Quantity = document.querySelector(".number").textContent;
    var medicationId = event.target.getAttribute("value");
    if (event.target.tagName === "A" && medicationId != "add") {
      try {
        var result = await PostData(
          "http://localhost:5033/api/Medication/AddMedicationItem",
          {
            medicationId: medicationId,
            medicineId: MedicineId,
            quantity: Quantity,
          },
          "PUT",
          token
        );
        $("#addmedication").modal("hide");
        MyMedication();
        setTimeout(() => {
          Toastify({
            text: " The Medication Added successfully",
            duration: 3000,
            gravity: "top",
            position: "right",
            style: {
              background: "#14da59",
            },
          }).showToast();
        }, 1);
        console.log(result);
      } catch (error) {}
    }
  });

document
  .getElementById("add-medication")
  .addEventListener("click", async () => {
    var medicationName = document.getElementById("medication-name").value;
    var medicationDescription = document.getElementById(
      "medication-description"
    ).value;
    console.log(medicationName, medicationDescription);
    var MedicationResult = await PostData(
      "http://localhost:5033/api/Medication/CreateMedication",
      {
        medicationName: medicationName,
        medicationDescription: medicationDescription,
      },
      "POST",
      token
    );

    $("#addmedication").modal("hide");
    MyMedication();
    setTimeout(() => {
      Toastify({
        text: " The Medication Added successfully",
        duration: 3000,
        gravity: "top",
        position: "right",
        style: {
          background: "#14da59",
        },
      }).showToast();
    }, 1);
  });

document.getElementById("add-cart").addEventListener("click", async () => {
  try {
    var Quantity = document.querySelector(".number").textContent;
    var addcart = await PostData(
      "http://localhost:5033/api/Cart/AddToCart",
      {
        medicineId: MedicineId,
        quantity: Quantity,
      },
      "POST",
      token
    );
    console.log(addcart);
    await InitializeData();
    setTimeout(() => {
      Toastify({
        text: " Item added to cart",
        duration: 3000,
        gravity: "top",
        position: "right",
        style: {
          background: "#14da59",
        },
      }).showToast();
    }, 1);
  } catch (error) {
    
    Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "red",
      },
    }).showToast();
  }
});

var starvalue;
document.querySelector(".rate").addEventListener("change", function (event) {
  starvalue = event.target.value;
});

document.getElementById("give-feedback").addEventListener("click", async () => {
  try {
    var heading = document.getElementById("review-head").value;
    var feedback = document.getElementById("review-body").value;

    console.log(heading);
    console.log(feedback);
    console.log(starvalue);
    const addFeedback = await PostData(
      "http://localhost:5033/api/Feedback/AddFeedback",
      {
        medicineId: MedicineId,
        title: heading,
        feedback: feedback,
        rating: starvalue,
      },
      "POST",
      token
    );

    $("#feedback-modal").modal("hide");
    await InitializeData();

    // Clear the form fields
    document.getElementById("review-head").value = "";
    document.getElementById("review-body").value = "";
    document.querySelector('.rate input[type="radio"]:checked').checked = false;

    setTimeout(() => {
      Toastify({
        text: "Thanks for sharing your feedback",
        duration: 3000,
        gravity: "top",
        position: "right",
        style: {
          background: "#14da59",
        },
      }).showToast();
    }, 1);
  } catch (error) {
    console.log(error);
    Toastify({
      text: "Error adding feedback. Please try again.",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989);",
      },
    }).showToast();
  }
});
