function toggleMenu() {
  let navigation = document.querySelector(".navigation");
  let nav = document.querySelector(".nav-cont");
  navigation.classList.toggle("active");
  nav.classList.toggle("active");
}
let allMedicines = [];
let currentPage = 1;
const itemsPerPage = 10;

async function fetchMedicines() {
  try {
    const response = await fetch(
      "http://localhost:5033/api/Admin/GetAllMedicine"
    );
    allMedicines = await response.json();
    console.log(allMedicines);
    displayMedicines();
    setupPagination();
  } catch (error) {
    console.error("Error fetching medicines:", error);
  }
}

function displayMedicines(medicines = allMedicines) {
  const tableBody = document.getElementById("productTableBody");
  tableBody.innerHTML = "";

  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const paginatedMedicines = medicines.slice(start, end);

  paginatedMedicines.forEach((medicine, index) => {
    const row = `
      <tr>
        <td>${start + index + 1}</td>
        <td>${medicine.medicineName}</td>
        <td>${medicine.categoryName}</td>
        <td>${medicine.brandname}</td>
        <td>${medicine.currentQuantity}</td>
        <td><div class="${medicine.status === 1 ? "approved" : "disable"}">${
      medicine.status === 1 ? "Active" : "Inactive"
    }</div></td>
        <td><button class="btn btn-outline-success">View</button></td>
      </tr>
    `;
    tableBody.innerHTML += row;
  });

  updatePaginationInfo(medicines.length);
}

function setupPagination() {
  const paginationContainer = document.querySelector(".pagination");
  paginationContainer.innerHTML = "";

  const totalPages = Math.ceil(allMedicines.length / itemsPerPage);

  // Previous button
  paginationContainer.innerHTML += `<button class="page-btn" onclick="changePage(${
    currentPage - 1
  })">&laquo;</button>`;

  // Page numbers
  for (let i = 1; i <= totalPages; i++) {
    paginationContainer.innerHTML += `<button class="page-btn ${
      i === currentPage ? "active" : ""
    }" onclick="changePage(${i})">${i}</button>`;
  }

  // Next button
  paginationContainer.innerHTML += `<button class="page-btn" onclick="changePage(${
    currentPage + 1
  })">&raquo;</button>`;
}

function changePage(page) {
  const totalPages = Math.ceil(allMedicines.length / itemsPerPage);
  if (page < 1 || page > totalPages) return;
  currentPage = page;
  displayMedicines();
  setupPagination();
}

function updatePaginationInfo(totalItems) {
  const start = (currentPage - 1) * itemsPerPage + 1;
  const end = Math.min(currentPage * itemsPerPage, totalItems);
  document.querySelector(
    ".pagination-info"
  ).textContent = `${start}-${end} of ${totalItems}`;
}

document.querySelector(".search input").addEventListener("input", (e) => {
  const searchTerm = e.target.value.toLowerCase();
  const filteredMedicines = allMedicines.filter(
    (medicine) =>
      medicine.medicineName.toLowerCase().includes(searchTerm) ||
      medicine.categoryName.toLowerCase().includes(searchTerm) ||
      medicine.brandname.toLowerCase().includes(searchTerm)
  );
  currentPage = 1;
  displayMedicines(filteredMedicines);
  setupPagination();
});

// Filter functionality
document
  .querySelector(".modal-footer .btn-primary")
  .addEventListener("click", () => {
    const sortWith = document.getElementById("sortWith").value;
    const sortBy = document.getElementById("sortBy").value;

    let filteredMedicines = [...allMedicines];

    if (sortWith !== "Sort with") {
      filteredMedicines.sort((a, b) => {
        if (sortWith === "Medicine Name") {
          return a.medicineName.localeCompare(b.medicineName);
        } else if (sortWith === "Price") {
          // Add price comparison logic if available
        } else if (sortWith === "Quantity") {
          return a.currentQuantity - b.currentQuantity;
        }
      });

      if (sortBy === "Descending") {
        filteredMedicines.reverse();
      }
    }

    currentPage = 1;
    displayMedicines(filteredMedicines);
    setupPagination();
    $("#filtermodal").modal("hide");
  });

// Initialize

document.onload = Validate();


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
    fetchMedicines();
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});
