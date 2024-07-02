import { GetData, PostData } from "../Api/Api.js";
var token =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNzIwNzc3MDY2fQ.7BxTYPoTyvdfGfS7hu0DYuBu1uBl-CasYQL8h8fr73Y";
window.onload = MyMedication();
async function MyMedication() {
  try {
    var Medication = await GetData(
      "http://localhost:5033/api/View/ViewMyMedications",
      {},
      token
    );
    console.log(Medication);
    CreateMedication(Medication);
  } catch (error) {
    console.log(error);
  }
}
function formatDate(dateString) {
  const date = new Date(dateString);
  const options = { year: "numeric", month: "long", day: "numeric" };
  return date.toLocaleDateString("en-US", options);
}

function CreateMedication(data) {
  var medicaioncont = document.getElementById("medication-cont");
  var content = "";

  data.forEach((element) => {
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
                 
                    <button class="view-btn" 
                    value=${element.medicationId}>View Medication</button>
                   
                  
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
    medicationName.value = "";
    medicationDescription.value = "";
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
    }, 1000);
  });
