import { GetData, PostData } from "../FetchApi/Api.js";

let rowCount = 0;

window.onload = Validate();

async function Validate() {
  try {
    var token =await localStorage.getItem("token");
    const validate = await fetch("http://localhost:5033/api/Auth/validate", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });

    if (!validate.ok) {
      const error = await response.json();
      throw new Error(error);
    }
    console.log(validate);

    var response = await validate.json();

    if (response.role != "Admin") {
      window.location.href = "../../Customer/Login/Login.html";
    }
  } catch (error) {
    window.location.href = "../../Customer/Login/Login.html";
  } finally {
    initializePage();
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});

async function initializePage() {
  await GetAllMedicine();
  addRow(); // Add initial empty row
}

async function GetAllMedicine() {
  var Medicine = await GetData(
    "http://localhost:5033/api/Admin/GetAllMedicine",
    {}
  );
  var Vendor = await GetData(
    "http://localhost:5033/api/Admin/GetAllVendor",
    {}
  );

  // Store the medicine and vendor data globally
  window.medicineData = Medicine;
  window.Vendor = Vendor;
}

function createRow(rowIndex) {
  return `
    <tr>
      <td>${rowIndex}</td>
      <td>
        <div class="custom-container">
          <select id="search-select${rowIndex}" class="form-select medicine-input" style="width: 100%">
            <option value="">Select Brand</option>
          </select>
        </div>
      </td>
      <td><input type="number" class="form-control quantity-input" placeholder="Quantity" required min="1"></td>
      <td><input type="number" class="form-control amount-input" placeholder="Amount" required min="0.01" step="0.01"></td>
      <td><input type="number" class="form-control total-input" placeholder="Total" readonly></td>
      <td>
        <div class="custom-container">
          <select id="vendor-select${rowIndex}" class="form-select vendor-input" style="width: 100%">
            <option value="">Select Vendor</option>
          </select>
        </div>
      </td>
      <td><input type="date" class="form-control exp-date-input" required></td>
    </tr>
  `;
}

function addRow() {
  rowCount++;
  const newRow = createRow(rowCount);
  document.getElementById("tableBody").insertAdjacentHTML("beforeend", newRow);

  initializeSelect2ForRow(rowCount);
  initializeVendorSelect(rowCount);
  initializeRowEventListeners(rowCount);
}

function initializeSelect2ForRow(rowIndex) {
  $(`#search-select${rowIndex}`)
    .select2({
      dropdownParent: $(`#search-select${rowIndex}`).closest(
        ".custom-container"
      ),
      data: window.medicineData.map((medicine) => ({
        id: medicine.medicineId,
        text: medicine.medicineName,
      })),
    })
    .on("select2:open", function () {
      setTimeout(() => {
        document.querySelector(".select2-search__field").focus();
      }, 100);
    });
}

function initializeVendorSelect(rowIndex) {
  $(`#vendor-select${rowIndex}`)
    .select2({
      dropdownParent: $(`#vendor-select${rowIndex}`).closest(
        ".custom-container"
      ),
      data: window.Vendor.map((vendor) => ({
        id: vendor.vendorId,
        text: vendor.vendorName,
      })),
    })
    .on("select2:open", function () {
      setTimeout(() => {
        document.querySelector(".select2-search__field").focus();
      }, 100);
    });
}

function initializeRowEventListeners(rowIndex) {
  const row = document.querySelector(`#tableBody tr:nth-child(${rowIndex})`);
  const quantityInput = row.querySelector(".quantity-input");
  const amountInput = row.querySelector(".amount-input");
  const totalInput = row.querySelector(".total-input");
  [quantityInput, amountInput].forEach((input) => {
    input.addEventListener("input", () =>
      calculateTotal(quantityInput, amountInput, totalInput)
    );
  });
}

function calculateTotal(quantityInput, amountInput, totalInput) {
  const quantity = parseFloat(quantityInput.value) || 0;
  const amount = parseFloat(amountInput.value) || 0;
  const total = quantity * amount;
  totalInput.value = total.toFixed(2);
}

function isRowEmpty(row) {
  const inputs = row.querySelectorAll("input:not([readonly])");
  const selects = row.querySelectorAll("select");
  return (
    Array.from(inputs).every((input) => input.value === "") &&
    Array.from(selects).every((select) => select.value === "")
  );
}

function isRowValid(row) {
  const medicineSelect = row.querySelector(".medicine-input");
  const quantityInput = row.querySelector(".quantity-input");
  const amountInput = row.querySelector(".amount-input");
  const vendorSelect = row.querySelector(".vendor-input");
  const expDateInput = row.querySelector(".exp-date-input");

  const quantity = parseFloat(quantityInput.value);
  const amount = parseFloat(amountInput.value);

  return (
    medicineSelect.value !== "" &&
    quantityInput.value !== "" &&
    amountInput.value !== "" &&
    vendorSelect.value !== "" &&
    expDateInput.value !== "" &&
    quantity > 0 &&
    amount > 0
  );
}

document
  .getElementById("tableBody")
  .addEventListener("input", async function (e) {
    const currentRow = e.target.closest("tr");
    const lastRow = this.lastElementChild;

    // If typing in the last row, add a new row
    if (currentRow === lastRow && !isRowEmpty(currentRow)) {
      addRow();
    }

    // Remove empty second-to-last row if exists
    if (this.children.length >= 2) {
      const secondToLastRow = this.children[this.children.length - 2];
      const lastRow = this.children[this.children.length - 1];
      if (isRowEmpty(secondToLastRow) && !isRowEmpty(lastRow)) {
        secondToLastRow.remove();
        rowCount--;
      }
    }
  });

function convertToDateTime(dateString) {
  const date = new Date(dateString);

  if (isNaN(date.getTime())) {
    return null;
  }

  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
}

function resetForm() {
  document.getElementById("tableBody").innerHTML = "";
  document.getElementById("purchaseDate").value = "";
  rowCount = 0;
  addRow(); // Add initial empty row
}

function showToast(message, isError = false) {
  Toastify({
    text: message,
    duration: 3000,
    gravity: "top",
    position: "right",
    style: {
      background: isError
        ? "rgba(241, 73, 47, 0.989)"
        : "rgba(54, 223, 16, 0.989)",
    },
  }).showToast();
}

document
  .getElementById("submitData")
  .addEventListener("click", async function () {
    const rows = document.querySelectorAll("#tableBody tr");
    var purchaseDate = document.getElementById("purchaseDate").value;

    if (!purchaseDate) {
      showToast("Please select a purchase date.", true);
      return;
    }

    var Totresult = { dateTime: convertToDateTime(purchaseDate), items: [] };
    const data = Totresult.items;
    let isValid = true;

    rows.forEach((row, index) => {
      if (rows.length - 1 != index) {
        if (!isRowValid(row)) {
          isValid = false;
          return;
        }

        const rowData = {
          medicineId:
            $(`#search-select${index + 1}`).select2("data")[0]?.id || "",
          vendorId:
            $(`#vendor-select${index + 1}`).select2("data")[0]?.id || "",
          amount: row.querySelector(".amount-input").value,
          quantity: row.querySelector(".quantity-input").value,
          expiryDate: convertToDateTime(
            row.querySelector(".exp-date-input").value
          ),
        };

        if (!rowData.medicineId || !rowData.vendorId || !rowData.expiryDate) {
          isValid = false;
          return;
        }

        data.push(rowData);
      }
    });

    if (!isValid) {
      showToast(
        "Please fill all fields correctly. Quantity and Amount must be greater than zero.",
        true
      );
      return;
    }

    if (data.length === 0) {
      showToast("Please add at least one valid row of data.", true);
      return;
    }

    try {
      console.log(Totresult);
      const response = await fetch("http://localhost:5033/api/Admin/Purchase", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(Totresult),
      });

      if (response.ok) {
        const FinalResult = await response.json();
        console.log(FinalResult);
        showToast("Purchase submitted successfully!");
        resetForm();
      } else {
        const errorData = await response.json();
        showToast(
          `Error: ${errorData.message || "Unknown error occurred"}`,
          true
        );
      }
    } catch (error) {
      console.error("An error occurred while fetching data:", error);
      showToast(`Error: ${error.message || "Unknown error occurred"}`, true);
    }
  });
