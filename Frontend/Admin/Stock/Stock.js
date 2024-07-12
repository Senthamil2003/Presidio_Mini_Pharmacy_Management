function toggleMenu() {
  let navigation = document.querySelector(".navigation");
  let nav = document.querySelector(".nav-cont");
  navigation.classList.toggle("active");
  nav.classList.toggle("active");
}
async function resetForm() {
  // Reset text inputs
  document.getElementById("medicine-name").value = "";
  document.getElementById("floatingTextarea").value = "";
  document.getElementById("sellingPrice").value = "";
  document.getElementById("item-count").value = "";
  document.getElementById("weight").value = "";

  // Reset file inputs
  document.getElementById("imageFile").value = "";
  document.getElementById("categoryimage").value = "";
  document.getElementById("brandimage").value = "";

  // Reset image preview
  document.getElementById("medicineImage").src =
    "../Assets/Images/medicine-bottle-and-pills-black-and-white-icon-illustration-vector.jpg";

  // Reset Select2 dropdowns
  $("#search-select1").val("0").trigger("change");
  $("#search-select2").val("0").trigger("change");

  // Reset regular select
  document.getElementById("status").selectedIndex = 0;

  // Clear any added options in the select dropdowns
  const select1 = document.getElementById("search-select1");
  const select2 = document.getElementById("search-select2");

  Array.from(select1.options).forEach((option) => {
    if (option.value === "-1") {
      select1.removeChild(option);
    }
  });

  Array.from(select2.options).forEach((option) => {
    if (option.value === "-1") {
      select2.removeChild(option);
    }
  });
}

function Photo(event) {
  const input = event.target;
  const preview = document.getElementById("medicineImage");

  const files = input.files;
  if (files && files.length > 0) {
    const file = files[0];
    const reader = new FileReader();
    reader.onload = function (event) {
      preview.src = event.target.result;
    };
    reader.readAsDataURL(file);
  } else {
    preview.src =
      "../Assets/Images/medicine-bottle-and-pills-black-and-white-icon-illustration-vector.jpg";
  }
}

async function populateSelects() {
  try {
    const categoryResponse = await fetch(
      "http://localhost:5033/api/Admin/GetAllCategory"
    );
    const categories = await categoryResponse.json();

    const brandResponse = await fetch(
      "http://localhost:5033/api/Admin/GetAllBrand"
    );
    const brands = await brandResponse.json();

    const brandSelect = document.getElementById("search-select1");
    const categorySelect = document.getElementById("search-select2");

    categories.forEach((category) => {
      const option = document.createElement("option");
      option.value = category.id;
      option.textContent = category.categoryName;
      categorySelect.appendChild(option);
    });

    brands.forEach((brand) => {
      const option = document.createElement("option");
      option.value = brand.id;
      option.textContent = brand.brandName;
      brandSelect.appendChild(option);
    });
    console.log(brands, categories);
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}

document.addEventListener("DOMContentLoaded", () => {
  Validate();
  populateSelects();
});

async function Submitdata(event) {
  event.preventDefault();

  const medicineName = document.getElementById("medicine-name").value;
  const brandId = document.getElementById("search-select1").value;
  const categoryId = document.getElementById("search-select2").value;
  const description = document.getElementById("floatingTextarea").value;
  const sellingPrice = document.getElementById("sellingPrice").value;
  const status = document.getElementById("status").value;
  const medicineImageFile = document.getElementById("imageFile").files[0];
  const itemPerPack = document.getElementById("item-count").value;
  const weigth = document.getElementById("weight").value;

  const brandName =
    document.getElementById("search-select1").options[
      document.getElementById("search-select1").selectedIndex
    ].text;
  const categoryName =
    document.getElementById("search-select2").options[
      document.getElementById("search-select2").selectedIndex
    ].text;
  const categoryimage = document.getElementById("categoryimage").files[0];
  const brandImageFile = document.getElementById("brandimage").files[0];

  console.log(brandId, categoryId);
  const isnewBrand = brandId === "-1";
  const isnewCategory = categoryId === "-1";

  // Validation
  let errors = [];

  if (!medicineName) errors.push("Medicine name is required.");
  if (brandId === "0") errors.push("Please select a brand.");
  if (categoryId === "0") errors.push("Please select a category.");
  if (!description) errors.push("Description is required.");
  if (!sellingPrice) errors.push("Selling price is required.");
  else if (isNaN(parseFloat(sellingPrice)) || parseFloat(sellingPrice) <= 0) {
    errors.push("Selling price must be a positive number.");
  }
  if (!status) errors.push("Please select a status.");
  if (!medicineImageFile) errors.push("Please upload a medicine image.");
  if (!itemPerPack) errors.push("Item per pack is required.");
  else if (!Number.isInteger(Number(itemPerPack)) || Number(itemPerPack) <= 0) {
    errors.push("Item per pack must be a positive integer.");
  }
  if (!weight) errors.push("Weight is required.");

  if (errors.length > 0) {
    // Display errors
    const errorMessage = errors.join("\n");
    await Toastify({
      text: errorMessage,
      duration: 5000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989)",
      },
    }).showToast();
    return; // Stop form submission if there are errors
  }

  // If validation passes, proceed with form submission
  const formData = new FormData();

  formData.append("medicineName", medicineName);
  formData.append("brandId", brandId);
  formData.append("categoryId", categoryId);
  formData.append("description", description);
  formData.append("sellingPrice", sellingPrice);
  formData.append("status", status);
  formData.append("brandName", brandName);
  formData.append("categoryName", categoryName);
  formData.append("isnewBrand", isnewBrand);
  formData.append("isnewCategory", isnewCategory);
  formData.append("weight", weigth);
  formData.append("itemPerPack", itemPerPack);
  if (brandImageFile) {
    formData.append("BrandImage", brandImageFile);
  }
  if (categoryimage) {
    formData.append("CategoryImage", categoryimage);
  }

  if (medicineImageFile) {
    formData.append("medicineImage", medicineImageFile);
  }

  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const response = await fetch(
      "http://localhost:5033/api/Admin/AddMedicine",
      {
        method: "POST",
        body: formData,
        headers: {
          Accept: "application/json",
        },
      }
    );

    if (!response.ok) {
      const error = await response.json();
      console.log(error);
      throw new Error(error.message);
    }
    await Toastify({
      text: "Medicine Added Successfully",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(54, 223, 16, 0.989)",
      },
    }).showToast();
    resetForm();
  } catch (error) {
    await Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989)",
      },
    }).showToast();
    console.error("Error submitting form:", error);
  } finally {
    spinner.style.visibility = "hidden";
  }
}

document
  .getElementById("addcategorybtn")
  .addEventListener("click", function () {
    const categoryName = document.getElementById("category").value;
    const categoryimage = document.getElementById("categoryimage").files[0];

    if (categoryName && categoryimage) {
      const option = document.createElement("option");
      option.value = "-1";
      option.textContent = categoryName;
      option.selected = true;
      document.getElementById("search-select2").appendChild(option);

      $("#addcatagory").modal("hide");
    }
  });

document.querySelector("#addbrandbtn").addEventListener("click", function () {
  console.log("click in brand");
  const brandName = document.getElementById("brand").value;
  const brandImageFile = document.getElementById("brandimage").files[0];

  if (brandName && brandImageFile) {
    const option = document.createElement("option");
    option.value = "-1";
    option.textContent = brandName;
    option.selected = true;
    document.getElementById("search-select1").appendChild(option);

    $("#addbrand").modal("hide");
  }
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
