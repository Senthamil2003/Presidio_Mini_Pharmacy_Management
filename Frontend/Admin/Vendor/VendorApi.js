import { GetData, PostData } from "../FetchApi/Api.js";

let currentPage = 1;
const itemsPerPage = 10;
let allVendors = [];

window.onload = Validate();

async function FetchVendor() {
  allVendors = await GetData(
    "http://localhost:5033/api/Admin/GetAllVendor",
    {}
  );
  updateTable();
  updatePagination();
}

function updateTable(vendors = allVendors) {
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentVendors = vendors.slice(startIndex, endIndex);

  var body = document.getElementById("vendor-table");
  var content = "";
  currentVendors.forEach((element, i) => {
    content += `<tr>
      <td>${startIndex + i + 1}</td>
      <td>${element.vendorName}</td>
      <td>${element.address}</td>
      <td>${element.phone}</td>
      <td>${element.mail}</td>
    </tr>`;
  });
  content += "<tr></tr>";
  body.innerHTML = content;

  updatePaginationInfo(vendors.length);
}

function updatePaginationInfo(totalItems) {
  const startItem = (currentPage - 1) * itemsPerPage + 1;
  const endItem = Math.min(currentPage * itemsPerPage, totalItems);
  document.querySelector(
    ".pagination-info"
  ).textContent = `${startItem}-${endItem} of ${totalItems}`;
}

function updatePagination() {
  const totalPages = Math.ceil(allVendors.length / itemsPerPage);
  let paginationHTML = `
    <button class="page-btn" onclick="changePage(${
      currentPage - 1
    })">&laquo;</button>
  `;

  for (let i = 1; i <= totalPages; i++) {
    paginationHTML += `
      <button class="page-btn ${
        i === currentPage ? "active" : ""
      }" onclick="changePage(${i})">${i}</button>
    `;
  }

  paginationHTML += `
    <button class="page-btn" onclick="changePage(${
      currentPage + 1
    })">&raquo;</button>
  `;

  document.querySelector(".pagination").innerHTML = paginationHTML;
}

window.changePage = function (page) {
  const totalPages = Math.ceil(allVendors.length / itemsPerPage);
  if (page < 1 || page > totalPages) return;
  currentPage = page;
  updateTable();
  updatePagination();
};

document.querySelector(".search input").addEventListener("input", function (e) {
  const searchTerm = e.target.value.toLowerCase();
  const filteredVendors = allVendors.filter(
    (vendor) =>
      vendor.vendorName.toLowerCase().includes(searchTerm) ||
      vendor.address.toLowerCase().includes(searchTerm) ||
      vendor.phone.toLowerCase().includes(searchTerm) ||
      vendor.mail.toLowerCase().includes(searchTerm)
  );
  currentPage = 1;
  updateTable(filteredVendors);
  updatePagination();
});

document.getElementById("add-vendor").addEventListener("click", async () => {
  var name = document.getElementById("vendorname").value;
  var phone = document.getElementById("phone").value;
  var mail = document.getElementById("mail").value;
  var address = document.getElementById("address").value;

  var params = {
    vendorName: name,
    address: address,
    phone: phone,
    mail: mail,
  };

  try {
    var result = await PostData(
      "http://localhost:5033/api/Admin/AddVendor",
      params,
      "Post"
    );
    console.log(result);
    Toastify({
      text: "Vendor Added Successfully",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(19, 236, 19, 0.989)",
      },
    }).showToast();
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

  $("#Addvendor").modal("hide");
  await FetchVendor();
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

    var response = await validate.json();
    console.log(response);

    if (response.role != "Admin") {
      window.location.href = "../../Customer/Login/Login.html";
    }
  } catch (error) {
    window.location.href = "../../Customer/Login/Login.html";
  } finally {
    FetchVendor();
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});
