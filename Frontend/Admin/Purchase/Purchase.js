function toggleMenu() {
  let navigation = document.querySelector(".navigation");
  let nav = document.querySelector(".nav-cont");
  navigation.classList.toggle("active");
  nav.classList.toggle("active");
}

let rowCount = 0;

function createRow() {
  rowCount++;
  return `
      <tr>
        <td>${rowCount}</td>
        <td><input class="form-control" list="datalistOptions" placeholder="Medicine.."></td>
        <td><input type="text" class="form-control" placeholder="Category"></td>
        <td><input class="form-control" list="datalistOptions" placeholder="Brand..."></td>
        <td><input type="text" class="form-control" placeholder="Describe"></td>
        <td><input type="number" class="form-control quantity" placeholder="Quantity"></td>
        <td><input type="number" class="form-control amount" placeholder="Amount"></td>
        <td><input type="text" class="form-control total" placeholder="Total" readonly></td>
        <td><input type="text" class="form-control" placeholder="Vendor"></td>
        <td><input type="date" class="form-control exp-date"></td>
        <td><input type="text" class="form-control" placeholder="Dosage"></td>
      </tr>
    `;
}

function addRow() {
  document
    .getElementById("tableBody")
    .insertAdjacentHTML("beforeend", createRow());
}

document.getElementById("tableBody").addEventListener("click", function (e) {
  const clickedRow = e.target.closest("tr");
  if (clickedRow && clickedRow === this.lastElementChild) {
    addRow();
  }
});

document.getElementById("tableBody").addEventListener("input", function (e) {
  if (
    e.target.classList.contains("quantity") ||
    e.target.classList.contains("amount")
  ) {
    const row = e.target.closest("tr");
    const quantity = parseFloat(row.querySelector(".quantity").value) || 0;
    const amount = parseFloat(row.querySelector(".amount").value) || 0;
    const total = quantity * amount;
    row.querySelector(".total").value = total.toFixed(2);
  }
});

document.getElementById("submitData").addEventListener("click", function () {
  const rows = document.querySelectorAll("#tableBody tr");
  const data = [];

  rows.forEach((row, index) => {
    if (index < rows.length - 1) {
      const rowData = {
        medicine: row.cells[1].querySelector("input").value,
        category: row.cells[2].querySelector("input").value,
        brand: row.cells[3].querySelector("input").value,
        description: row.cells[4].querySelector("input").value,
        quantity: row.cells[5].querySelector("input").value,
        amount: row.cells[6].querySelector("input").value,
        total: row.cells[7].querySelector("input").value,
        vendor: row.cells[8].querySelector("input").value,
        expDate: row.cells[9].querySelector("input").value,
        dosage: row.cells[10].querySelector("input").value,
      };
      data.push(rowData);
    }
  });

  console.log(data);
});

addRow();
