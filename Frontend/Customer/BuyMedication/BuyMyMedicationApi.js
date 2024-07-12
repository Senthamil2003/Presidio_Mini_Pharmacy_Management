var token = localStorage.getItem("token");
var MedicationId;

document.onload = Validate();

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

function getQueryParam(param) {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get(param);
}

async function MyMedication() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    MedicationId = getQueryParam("medication");
    const response = await fetch(
      `http://localhost:5033/api/View/GetMedicationItems?medicationId=${MedicationId}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
      }
    );
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const error = await response.json();
      console.log(error);
      throw new Error(error);
    }

    const Medication = await response.json();

    console.log(Medication);
    CreateElement(
      Medication.medicationItemDetailDTOs,
      Medication.medicationName
    );
  } catch (error) {
    spinner.style.visibility = "hidden";
    document.querySelector(".no-item-found").style.visibility = "visible";
    document.querySelector(".cart").style.display = "none";
    console.error("Error fetching medication data:", error);
  }
}

function CreateElement(data, name) {
  var count = 0;
  var medicineCt = 0;
  var content = "";
  var Container = document.getElementById("medication-cont");
  var medicationName = document.getElementById("medication-name");
  var total = document.getElementById("item-count");
  var cost = document.getElementById("cost");
  var medicinect = document.getElementById("medicine-count");
  data.forEach((element) => {
    count += Number(element.quantity) * Number(element.amount);
    medicineCt += element.quantity;

    content += `   <div class="card dark">
         <div class="img-cont">
            <img
              src="data:image/jpeg;base64,${element.image}"
              class="card-img-top"
              alt="..."
            />
            </div>
            <div class="card-body">
              <span class="delete-btn" value=${element.medicationItemId} onclick="Delete(event)">&times;</span>
              <div class="text-section">
                <h5 class="card-title fw-bold">${element.medicineName}</h5>
                <p class="card-text company">${element.brandName}</p>
                <div class="card-cont">
                  <p class="card-text">Weight:${element.weight}</p>
                  <p class="card-text">Pack of ${element.itemPerPack}-item</p>
                </div>
              </div>
              <div class="cta-section">
                <div class="price">$${element.amount}</div>
                <div class="quantity-field">
                  <button
                    class="value-button decrease-button"
                    value=${element.medicationItemId}
                    onclick="decreaseValue(this)"
                    title="Azalt"
                  >
                    -
                  </button>
                  <div class="number">${element.quantity}</div>
                  <button
                  value=${element.medicationItemId}
                    class="value-button increase-button"
                    onclick="increaseValue(this, 10)"
                    title="Arrtır"
                  >
                    +
                  </button>
                </div>
              </div>
            </div>
          </div>`;
  });

  cost.innerHTML = count;
  total.innerHTML = data.length;
  medicationName.innerHTML = name;
  medicinect.innerHTML = medicineCt;
  Container.innerHTML = content;
}

async function increaseValue(button, limit) {
  var spinner = document.querySelector(".custom-spinner");
  const numberInput = button.parentElement.querySelector(".number");
  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (limit && value >= limit) {
    spinner.style.visibility = "hidden";
    return;
  }
  spinner.style.visibility = "visible";
  try {
    params = {
      medicationId: MedicationId,
      medicationItemId: button.value,
      quantity: 1,
      status: "Increase",
    };

    console.log(params);
    const response = await fetch(
      "http://localhost:5033/api/Medication/UpdateMedication",
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
        body: JSON.stringify(params),
      }
    );
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }
    await MyMedication();
    numberInput.innerHTML = value + 1;

    const data = await response.json();
    console.log("Data posted successfully:", data);
  } catch (error) {
    spinner.style.visibility = "hidden";

    console.error("An error occurred while posting data:", error.message);
  }
}

async function decreaseValue(button) {
  var spinner = document.querySelector(".custom-spinner");
  spinner.style.visibility = "visible";
  const numberInput = button.parentElement.querySelector(".number");
  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (value <= 1) {
    spinner.style.visibility = "hidden";
    return;
  }
  try {
    params = {
      medicationId: MedicationId,
      medicationItemId: button.value,
      quantity: 1,
      status: "Decrease",
    };

    console.log(params);
    const response = await fetch(
      "http://localhost:5033/api/Medication/UpdateMedication",
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
        body: JSON.stringify(params),
      }
    );
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }
    numberInput.innerHTML = value - 1;
    await MyMedication();

    const data = await response.json();
    console.log("Data posted successfully:", data);
  } catch (error) {
    spinner.style.visibility = "hidden";

    console.error("An error occurred while posting data:", error.message);
  }
}
async function Delete(event) {
  try {
    const value = event.target.getAttribute("value");
    console.log(value);

    const response = await fetch(
      `http://localhost:5033/api/Medication/RemoveMedicationItem?medicationId=${MedicationId}&medicationItemId=${value}`,
      {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
      }
    );

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }
    var result = await response.json();
    console.log(result);
    Toastify({
      text: "Item Removed Successfull",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "#14da59",
      },
    }).showToast();
    await MyMedication();
  } catch (error) {
    Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989)",
      },
    }).showToast();
  }
}

async function Checkout() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const response = await fetch(
      `http://localhost:5033/api/Medication/BuyMedication?medicationId=${MedicationId}`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
      }
    );
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(`${errorResponse.message || response.statusText}`);
    }

    const data = await response.json();
    Toastify({
      text: "Medication Bought sucessful",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "#14da59",
      },
    }).showToast();

    await MyMedication();
    console.log("Data posted successfully:", data);
  } catch (error) {
    spinner.style.visibility = "hidden";
    Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989)",
      },
    }).showToast();
    console.error("An error occurred while posting data:", error.message);
  }
}
