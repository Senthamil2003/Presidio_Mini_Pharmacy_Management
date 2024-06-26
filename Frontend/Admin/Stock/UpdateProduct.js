import { GetData, PostData } from "../FetchApi/Api.js";

document
  .getElementById("form-data")
  .addEventListener("submit", (event) => event.preventDefault());
document.getElementById("submit-btn").addEventListener("click", UpdateData);
document.getElementById("edit-btn").addEventListener("click", editForm);

window.onload = await displayMedicineImage();

async function UpdateData() {
  var imageFile = document.getElementById("imageFile").files[0];
  var medicineName = document.getElementById("medicine-name").value;
var brand = document.getElementById("brand").value;
var category = document.getElementById("category").value;
var description = document.getElementById("floatingTextarea").value;
var status = document.getElementById("status").value;
var sellingPrice = document.getElementById("sellingPrice").value;
var recentPurchasePrice = document.getElementById("recentPurchasePrice").value;


  let formData = new FormData();
  formData.append("MedicineId", 7);
  formData.append("MedicineName", medicineName);
  formData.append("Category", category);
  formData.append("Description", description);
  formData.append("SellingPrice", sellingPrice);
  formData.append("File", imageFile);
  formData.append("Status", status);
 

  console.log("FormData contents:", Object.fromEntries(formData));

  try {
    const response = await fetch(
      "http://localhost:5033/api/Admin/UpdateMedicine",
      {
        method: "PUT",
        body: formData,
      }
    );

    if (!response.ok) {
      const errorBody = await response.text();
      console.error(
        `HTTP error! Status: ${response.status}, Body: ${errorBody}`
      );
      throw new Error(`HTTP error! Status: ${response.status}`);
    }

    const result = await response.json();
    console.log(result);
  } catch (error) {
    console.error("Error:", error);
  }
}
async function displayMedicineImage() {
  try {
    const url = "http://localhost:5033/api/Admin/GetMedicine";
    var params = {
      MedicineId: 7,
    };
    var result = await GetData(url, params);
    if (result) {
      // Set image
      if (result.imageBase64) {
        const imgElement = document.getElementById("medicineImage");
        imgElement.src = `data:image/jpeg;base64,${result.imageBase64}`;
      } else {
        console.log("No image available for this medicine");
      }

      // Populate form fields and disable them
      const fieldsToPopulate = [
        "medicine-name",
        "brand",
        "category",
        "floatingTextarea",
        "status",
        "sellingPrice",
        "recentPurchasePrice",
      ];

      fieldsToPopulate.forEach((fieldId) => {
        const field = document.getElementById(fieldId);
        if (field) {
          switch (fieldId) {
            case "medicine-name":
              field.value = result.medicineName;
              break;
            case "brand":
              field.value = result.brand;
              break;
            case "category":
              field.value = result.categoryName;
              break;
            case "floatingTextarea":
              field.value = result.description;
              break;
            case "status":
              if (result.status === 0) {
                const pendingOption = document.createElement("option");
                pendingOption.value = "0";
                pendingOption.textContent = "Pending";
                field.insertBefore(pendingOption, field.firstChild);
              }
            

              field.value = result.status.toString();
              break;
            case "sellingPrice":
              field.value = result.sellingPrice;
              break;
            case "recentPurchasePrice":
              field.value = result.recentSellingPrice;
              break;
          }
          field.disabled = true;
        }
      });

      document.getElementById("imageFile").disabled = true;
      document.getElementById("submit-btn").disabled = true;
    }
  } catch (error) {
    console.error("Error:", error);
  }
}

function editForm() {
  
  const fieldsToEnable = [
    "medicine-name",
    "brand",
    "category",
    "floatingTextarea",
    "status",
    "sellingPrice",
    "imageFile",
  ];

  fieldsToEnable.forEach((fieldId) => {
    const field = document.getElementById(fieldId);
    if (field) {
      field.disabled = false;

     
      if (fieldId === "status") {
        const pendingOption = field.querySelector('option[value="0"]');
        if (pendingOption) {
          field.removeChild(pendingOption);
        }
        
        if (field.value === "0") {
          field.value = field.options[0].value;
        }
      }
    }
  });


  document.getElementById("submit-btn").disabled = false;
}


