import { GetData, PostData } from "../FetchApi/Api.js";

let rowCount = 0;

window.onload = initializePage();

async function initializePage() {
  await GetAllMedicine();
  renderDataFromLocalStorage();

}

async function GetAllMedicine() {
  var Medicine = await GetData(
    "http://localhost:5033/api/Admin/GetAllMedicine",
    {}
  );

  // Store the medicine data globally
  window.medicineData = Medicine;
}

function createRow(rowIndex, rowData = null) {
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
      <td><input type="number" class="form-control quantity-input" placeholder="Quantity" required min="1" value="${rowData ? rowData.quantity : ''}"></td>
      <td><input type="number" class="form-control amount-input" placeholder="Amount" required min="0.01" step="0.01" value="${rowData ? rowData.amount : ''}"></td>
      <td><input type="number" class="form-control total-input" placeholder="Total" disabled value="${rowData ? rowData.total : ''}"></td>
      <td><input type="text" class="form-control vendor-input" placeholder="Vendor" required value="${rowData ? rowData.vendor : ''}"></td>
      <td><input type="date" class="form-control exp-date-input" required value="${rowData ? rowData.expDate : ''}"></td>
    </tr>
  `;
}

function addRow(rowData = null) {
  rowCount++;
  const newRow = createRow(rowCount, rowData);
  document
    .getElementById("tableBody")
    .insertAdjacentHTML("beforeend", newRow);

  initializeSelect2ForRow(rowCount, rowData ? rowData.medicine : null);
}

function initializeSelect2ForRow(rowIndex, selectedMedicine = null) {
  $(`#search-select${rowIndex}`)
    .select2({
      dropdownParent: $(".custom-container").eq(rowIndex - 1),
      data: window.medicineData.map(medicine => ({
        id: medicine.categoryId,
        text: medicine.medicineName
      }))
    })
    .on("select2:open", function () {
      setTimeout(function () {
        document.querySelector(".select2-search__field").focus();
      }, 100);
    });

  if (selectedMedicine) {
    $(`#search-select${rowIndex}`).val(selectedMedicine).trigger('change');
  }
}

function isRowEmpty(row) {
  const inputs = row.querySelectorAll("input:not([readonly]):not([disabled])");
  const select = row.querySelector("select");
  return Array.from(inputs).every((input) => input.value === "") && select.value === "";
}

function isRowValid(row) {
  const inputs = row.querySelectorAll("input:not([readonly]):not([disabled])");
  const select = row.querySelector("select");
  const quantity = parseFloat(row.querySelector(".quantity-input").value);
  const amount = parseFloat(row.querySelector(".amount-input").value);

  return (
    Array.from(inputs).every((input) => input.value !== "") &&
    select.value !== "" &&
    quantity > 0 &&
    amount > 0
  );
}

function saveDataToLocalStorage() {
  const rows = document.querySelectorAll("#tableBody tr");
  const data = [];

  rows.forEach((row, index) => {
    const rowData = {
      medicine: $(`#search-select${index + 1}`).select2('data')[0].text,
      quantity: row.querySelector(".quantity-input").value,
      amount: row.querySelector(".amount-input").value,
      total: row.querySelector(".total-input").value,
      vendor: row.querySelector(".vendor-input").value,
      expDate: row.querySelector(".exp-date-input").value,
    };
    data.push(rowData);
  });

  localStorage.setItem("medicineData", JSON.stringify(data));
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
      if (isRowEmpty(secondToLastRow)) {
        lastRow.remove();
        rowCount--;
      }
    }

    // Save data to local storage after each input
    saveDataToLocalStorage();
  });

document.getElementById("submitData").addEventListener("click", function () {
  const rows = document.querySelectorAll("#tableBody tr");
  const data = [];
  let isValid = true;

  rows.forEach((row, index) => {
    if (!isRowValid(row)) {
      isValid = false;
      return;
    }

    const rowData = {
      medicine: $(`#search-select${index + 1}`).select2('data')[0].text,
      quantity: row.querySelector(".quantity-input").value,
      amount: row.querySelector(".amount-input").value,
      total: row.querySelector(".total-input").value,
      vendor: row.querySelector(".vendor-input").value,
      expDate: row.querySelector(".exp-date-input").value,
    };
    data.push(rowData);
  });

  if (!isValid) {
    alert(
      "Please fill all fields correctly. Quantity and Amount must be greater than zero."
    );
    return;
  }

  console.log(data);
  // Here you can send the data to your backend
});

function renderDataFromLocalStorage() {
  const savedData = localStorage.getItem('medicineData');
  if (savedData) {
    const data = JSON.parse(savedData);
    document.getElementById("tableBody").innerHTML = ''; // Clear existing rows
    rowCount = 0; // Reset row count
    data.forEach((rowData) => {
      addRow(rowData);
    });
  } else {
    addRow(); // Add an empty row if no data in local storage
  }
}

function setupClearButton() {
  const clearButton = document.createElement('button');
  clearButton.textContent = 'Clear Data';
  clearButton.className = 'btn btn-danger mt-3';
  clearButton.addEventListener('click', clearLocalStorage);
  document.body.appendChild(clearButton);
}

function clearLocalStorage() {
  localStorage.removeItem('medicineData');
  document.getElementById("tableBody").innerHTML = '';
  rowCount = 0;
  addRow(); // Add an empty row after clearing
  alert('Data cleared from local storage');
}