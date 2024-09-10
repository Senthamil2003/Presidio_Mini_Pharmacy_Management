import { GetData, PostData } from "../Api/Api.js";
var token = localStorage.getItem("token");
var Medication;
window.onload = Validate();
async function Validate() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const response = await fetch("http://localhost:5033/api/Auth/validate", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error);
    }
    const data = await response.json();
    console.log(data);

    await MyMedication();
  } catch (error) {
    console.log(error.message);
    window.location.href = "../Login/Login.html";
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});
async function MyMedication() {
  try {
    Medication = await GetData(
      "http://localhost:5033/api/View/ViewMyMedications",
      {},
      token
    );
    console.log(Medication);
    CreateMedication();
  } catch (error) {
    var medicaioncont = document.getElementById("medication-cont");
    medicaioncont.innerHTML = `<p class="empty-medication">No Medication Found</p>`;
    console.log(error);
  } finally {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "hidden";
  }
}
function formatDate(dateString) {
  const date = new Date(dateString);
  const options = { year: "numeric", month: "long", day: "numeric" };
  return date.toLocaleDateString("en-US", options);
}
function FilterData(data) {
  var search = document.getElementById("search").value.toLowerCase();
  return data.filter((element) =>
    element.medicationName.toLowerCase().includes(search)
  );
}

document
  .getElementById("button-addon2")
  .addEventListener("click", CreateMedication);

function CreateMedication() {
  var medicaioncont = document.getElementById("medication-cont");
  var content = "";
  var search = document.getElementById("search").value;
  var filterData;
  if (search == "") {
    filterData = Medication;
  } else {
    filterData = FilterData(Medication);
  }

  filterData.forEach((element) => {
    content += ` <div class="col-lg-6">
          <div class="card dark">
            <img
              src="../Assets/Images/medicine.webp"
              class="card-img-top"
              alt="..."
            />
            <div class="card-body">
              <span class="delete-btn" value=${
                element.medicationId
              }>&times;</span>
              <div class="text-section">
                <h5 class="card-title fw-bold">${element.medicationName}</h5>
                <p class="card-text company">
                Created on ${formatDate(element.createdDate)}</p>
                <hr />

                <p class="card-text">
                  ${element.description}
                </p>
                <div class="card-foot">
                  <p>Item Count: ${element.totalCount}</p>
                 <a href="http://127.0.0.1:5501/Customer/BuyMedication/BuyMedication.Html?medication=${
                   element.medicationId
                 }">
                    <button class="view-btn btn" 
                    value=${element.medicationId}>View Medication</button>
                    </a>
                   
                  
                </div>
              </div>
            </div>
          </div>
        </div>`;
  });
  medicaioncont.innerHTML = content;
}
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
    await MyMedication();
    $("#addmedication").modal("hide");
    MyMedication();
    document.getElementById("medication-description").value = "";
    document.getElementById("medication-name").value = "";
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
    }, 500);
  });

// document.querySelector(".delete-btn").addEventListener("click",DeleteMedication)
//   async function DeleteMedication(event){
//     console.log(event.tar)

//     // var MedicationResult = await PostData(
//     //   `http://localhost:5033/api/Medication/RemoveMedication?medicationId=`,
//     //   {
//     //     medicationName: medicationName,
//     //     medicationDescription: medicationDescription,
//     //   },
//     //   "POST",
//     //   token
//     // );

//   }.addEventListener('DOMContentLoaded', function() {
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});
document
  .getElementById("medication-cont")
  .addEventListener("click", async function (event) {
    if (event.target.classList.contains("delete-btn")) {
      var medicatioId = event.target.getAttribute("value");
      try {
        var MedicationResult = await PostData(
          `http://localhost:5033/api/Medication/RemoveMedication?medicationId=${medicatioId}`,
          {},
          "DELETE",
          token
        );
        await MyMedication();
        setTimeout(() => {
          Toastify({
            text: " The Medication Removed successfull",
            duration: 3000,
            gravity: "top",
            position: "right",
            style: {
              background: "#14da59",
            },
          }).showToast();
        }, 500);
        console.log(MedicationResult);
      } catch (error) {
        console.log(error);
      }
    }
  });
