function toggleMenu() {
  let navigation = document.querySelector(".navigation");
  let nav = document.querySelector(".nav-cont");
  navigation.classList.toggle("active");
  nav.classList.toggle("active");
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
    preview.src = "";
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

document.addEventListener("DOMContentLoaded", populateSelects);

async function Submitdata(event) {
  event.preventDefault();

  // Get all values
  const medicineName = document.getElementById("medicine-name").value;
  const brandId = document.getElementById("search-select1").value;
  const categoryId = document.getElementById("search-select2").value;
  const description = document.getElementById("floatingTextarea").value;
  const sellingPrice = document.getElementById("sellingPrice").value;
  const status = document.getElementById("status").value;
  const medicineImageFile = document.getElementById("imageFile").files[0];
  const itemPerPack =document.getElementById("item-count").value;
  const weigth=document.getElementById("weight").value;


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

  console.log("medicineName:", medicineName);
  console.log("brandId:", brandId);
  console.log("categoryId:", categoryId);
  console.log("description:", description);
  console.log("sellingPrice:", sellingPrice);
  console.log("status:", status);
  console.log("medicineImageFile:", medicineImageFile);
  console.log("brandName:", brandName);
  console.log("categoryName:", categoryName);
  console.log("isnewBrand:", isnewBrand);
  console.log("isnewCategory:", isnewCategory);
  console.log(weigth,itemPerPack)

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
  formData.append("weight",weigth);
  formData.append("itemPerPack",itemPerPack);
  if (brandImageFile) {
    formData.append("BrandImage", brandImageFile);
  }
  if (categoryimage) {
    formData.append("CategoryImage", categoryimage);
  }

  if (medicineImageFile) {
    formData.append("medicineImage", medicineImageFile);
  }
  // console.log("-----------------------------");

  // for (var [key, value] of formData.entries()) {
  //   console.log(key, value);
  // }

  try {
    console.log("Sending FormData:", formData);
    const response = await fetch(
      "http://localhost:5033/api/Admin/AddMedicine",
      {
        method: "POST",
        body: formData,
        headers: {
          Accept: "application/json",
          // Do not set Content-Type when using FormData
        },
      }
    );

    if (response.ok) {
      console.log("Form submitted successfully");
      const result = await response.json();
      console.log("Server response:", result);
      // Handle successful submission (e.g., show a success message, reset form)
    } else {
      console.error("Form submission failed");
      console.error("Status:", response.status);
      console.error("Status Text:", response.statusText);
      const errorText = await response.text();
      console.error("Error details:", errorText);
      // Handle submission failure
    }
  } catch (error) {
    console.error("Error submitting form:", error);
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
