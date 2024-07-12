let allOrders = [];
let currentPage = 1;
const ordersPerPage = 10;

document.addEventListener("DOMContentLoaded", () => {
  Validate();
  fetchOrders();
  setupEventListeners();
});

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

    var response = await validate.json();

    if (response.role != "Admin") {
      window.location.href = "../../Customer/Login/Login.html";
    }
  } catch (error) {
    window.location.href = "../../Customer/Login/Login.html";
    console.log(error);
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});

function setupEventListeners() {
  const searchInput = document.querySelector(".search input");
  if (searchInput) {
    searchInput.addEventListener("input", handleSearch);
  }

  const filterButtons = document.querySelectorAll(".report-btn");
  filterButtons.forEach((button) => {
    button.addEventListener("click", handleFilter);
  });
}

async function fetchOrders() {
  try {
    const response = await fetch(
      "http://localhost:5033/api/Admin/GetAllOrders"
    );
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    allOrders = await response.json();
    displayOrders();
    console.log(allOrders);
  } catch (error) {
    console.error("Error fetching orders:", error);
  }
}

function displayOrders(orders = allOrders, page = currentPage) {
  const tableBody = document.getElementById("orderTableBody");
  if (!tableBody) {
    console.error("Table body element not found");
    return;
  }

  tableBody.innerHTML = "";

  const totalPages = Math.ceil(orders.length / ordersPerPage);
  currentPage = Math.min(Math.max(1, page), totalPages);

  const startIndex = (currentPage - 1) * ordersPerPage;
  const endIndex = startIndex + ordersPerPage;
  const paginatedOrders = orders.slice(startIndex, endIndex);

  paginatedOrders.forEach((order, index) => {
    const row = `
        <tr>
          <td>${startIndex + index + 1}</td>
          <td>${order.orderDetailId}</td>
          <td>${order.customerid}</td>
          <td>${order.medicineName}</td>
          <td>${order.quantity}</td>
          <td>${formatDate(order.date)}</td>
          <td>
            ${
              order.status
                ? '<button class="btn btn-success" disabled>Delivered</button>'
                : `<button class="btn btn-warning" id="${order.orderDetailId}" onclick="Deliver(event)">Dispatch item</button>`
            }
          </td>
        </tr>
      `;
    tableBody.innerHTML += row;
  });

  updatePagination(orders.length);
}

function updatePagination(totalOrders) {
  const totalPages = Math.ceil(totalOrders / ordersPerPage);
  const paginationEl = document.querySelector(".pagination");
  const paginationInfoEl = document.querySelector(".pagination-info");

  if (paginationEl) {
    paginationEl.innerHTML = `
        <button class="page-btn" onclick="changePage(${
          currentPage - 1
        })">&laquo;</button>
        ${Array.from({ length: totalPages }, (_, i) => i + 1)
          .map(
            (page) =>
              `<button class="page-btn ${
                page === currentPage ? "active" : ""
              }" onclick="changePage(${page})">${page}</button>`
          )
          .join("")}
        <button class="page-btn" onclick="changePage(${
          currentPage + 1
        })">&raquo;</button>
      `;
  } else {
    console.error("Pagination element not found");
  }

  if (paginationInfoEl) {
    const startIndex = (currentPage - 1) * ordersPerPage;
    const endIndex = Math.min(startIndex + ordersPerPage, totalOrders);
    paginationInfoEl.textContent = `${
      startIndex + 1
    }-${endIndex} of ${totalOrders}`;
  } else {
    console.error("Pagination info element not found");
  }
}

function changePage(page) {
  const filteredOrders = getCurrentFilteredOrders();
  displayOrders(filteredOrders, page);
}

function handleSearch(e) {
  const filteredOrders = getCurrentFilteredOrders();
  displayOrders(filteredOrders, 1); // Always start at page 1 for new searches
}

function handleFilter(event) {
  const button = event.target;

  document
    .querySelectorAll(".report-btn")
    .forEach((btn) => btn.classList.remove("active"));

  button.classList.add("active");

  const filteredOrders = getCurrentFilteredOrders();
  displayOrders(filteredOrders, 1); // Always start at page 1 for new filters
}

function getCurrentFilteredOrders() {
  const searchTerm = document
    .querySelector(".search input")
    .value.toLowerCase();
  const activeFilter = document
    .querySelector(".report-btn.active")
    .innerHTML.trim();

  let filteredOrders = allOrders;

  if (searchTerm) {
    filteredOrders = filteredOrders.filter(
      (order) =>
        order.medicineName.toLowerCase().includes(searchTerm) ||
        order.orderDetailId.toString().includes(searchTerm) ||
        order.customerid.toString().includes(searchTerm)
    );
  }

  if (activeFilter === "Dispatch") {
    filteredOrders = filteredOrders.filter((order) => order.status === false);
  }

  return filteredOrders;
}
function formatDate(dateString) {
  var date = new Date(dateString);
  var day = date.getDate().toString().padStart(2, "0");
  var month = (date.getMonth() + 1).toString().padStart(2, "0");
  var year = date.getFullYear();
  return `${day}/${month}/${year}`;
}

async function Deliver(event) {
  try {
    console.log(event.target.id);
    const response = await fetch(
      `http://localhost:5033/api/Admin/DeliverOrder?orderDetailId=${event.target.id}`
    );
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    var deliver = await response.json();
    await fetchOrders();
    console.log(deliver);
  } catch (error) {
    console.error("Error delivering order:", error);
  }
}
