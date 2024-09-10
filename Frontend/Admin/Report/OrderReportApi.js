import { GetData, PostData } from "../FetchApi/Api.js";

let allReportData = [];
let currentPage = 1;
const itemsPerPage = 10;

async function getDashboardData() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const url = "http://localhost:5033/api/Admin/GetDashboardData";
    const dashboardData = await GetData(url);
    updateDashboard(dashboardData);
  } catch (error) {
    console.log(error);
  } finally {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "hidden";
  }
}

function updateDashboard(data) {
  const updateElement = (id, value) => {
    const element = document.getElementById(id);
    if (element) {
      element.textContent = value;
    } else {
      console.warn(`Element with id '${id}' not found`);
    }
  };

  updateElement("customerCount", data.customerCount);
  updateElement("purchaseAmount", `$${data.purchaseAmount}`);
  updateElement("ordersAmount", `$${data.ordersAmount}`);
  updateElement("medicineCount", data.medicineCount);
}

function getPast30Days() {
  const toDate = new Date();
  const fromDate = new Date();
  fromDate.setDate(toDate.getDate() - 100);
  const formatDate = (date) => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    const seconds = String(date.getSeconds()).padStart(2, "0");
    const milliseconds = String(date.getMilliseconds()).padStart(3, "0");
    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${milliseconds}Z`;
  };
  return {
    fromDate: formatDate(fromDate),
    toDate: formatDate(toDate),
  };
}

async function fetchReportData() {
  try {
    var { fromDate, toDate } = getPast30Days();
    const params = {
      startDate: fromDate,
      endDate: toDate,
    };

    const url = "http://localhost:5033/api/Report/OrderReport";
    allReportData = await GetData(url, params);
    displayReport();
    setupPagination();
  } catch (error) {
    console.log(error);
  } finally {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "hidden";
  }
}
function formatDateToISO(date) {
  date = new Date(date);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  const seconds = String(date.getSeconds()).padStart(2, "0");
  const milliseconds = String(date.getMilliseconds()).padStart(3, "0");
  
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${milliseconds}Z`;
}

async function fetchFilteredReportData(fromDate, toDate) {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";

    const params = {
      startDate: formatDateToISO(fromDate),
      endDate: formatDateToISO(toDate),
    };
    console.log(params);

    const url = "http://localhost:5033/api/Report/OrderReport";
    allReportData = await GetData(url, params);
    displayReport();
    setupPagination();
  } catch (error) {
    allReportData = [];
    displayReport();
    setupPagination();
    console.log(error);
  } finally {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "hidden";
  }
}

function displayReport(data = allReportData) {
  var TotalContainer = document.getElementById("purchase-report");
  var content = "";
  const start = (currentPage - 1) * itemsPerPage;
  const end = start + itemsPerPage;
  const paginatedData = data.slice(start, end);

  paginatedData.forEach((element, i) => {
    content += ` <tr>
                <td>${start + i + 1}</td>
                <td>${element.medicineId}</td>
                <td>${element.medicineName}</td>
                <td>${element.totalQuantity}</td>
                <td>${element.totalAmount}</td>
              </tr>`;
  });
  TotalContainer.innerHTML = content;
  updatePaginationInfo(data.length);
}

function setupPagination() {
  const paginationContainer = document.querySelector(".pagination");
  paginationContainer.innerHTML = "";

  const totalPages = Math.ceil(allReportData.length / itemsPerPage);

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

  paginationContainer.innerHTML += `<button class="page-btn" onclick="changePage(${
    currentPage + 1
  })">&raquo;</button>`;
}

function changePage(page) {
  const totalPages = Math.ceil(allReportData.length / itemsPerPage);
  if (page < 1 || page > totalPages) return;
  currentPage = page;
  displayReport();
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
  const filteredData = allReportData.filter(
    (item) =>
      item.medicineName.toLowerCase().includes(searchTerm) ||
      item.medicineId.toString().includes(searchTerm)
  );
  currentPage = 1;
  displayReport(filteredData);
  setupPagination();
});

document
  .querySelector(".modal-footer .btn-primary")
  .addEventListener("click", () => {
    const fromDate = document.getElementById("fromDate").value;
    const toDate = document.getElementById("toDate").value;

    if (fromDate && toDate) {
      // If date range is selected, fetch new data
      fetchFilteredReportData(fromDate, toDate);
    } else {
      // If no date range, use existing data
      let filteredData = [...allReportData];

      currentPage = 1;
      displayReport(filteredData);
      setupPagination();
    }

    $("#exampleModal").modal("hide");
  });

document
  .querySelector(".modal-footer .btn-secondary")
  .addEventListener("click", () => {
    document.getElementById("fromDate").value = "";
    document.getElementById("toDate").value = "";

    currentPage = 1;

    displayReport();
    setupPagination();
  });

async function Validate() {
  try {
    var token = await localStorage.getItem("token");
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
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});

async function init() {
  await Validate();
  await getDashboardData();
  await fetchReportData();
}

init();
