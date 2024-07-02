var token =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjMiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNzIwNzc3MDY2fQ.7BxTYPoTyvdfGfS7hu0DYuBu1uBl-CasYQL8h8fr73Y";

document.onload = Initialize();
async function Initialize() {
  var spinner = document.querySelector(".custom-spinner");
  spinner.style.visibility = "visible";
  try {
    const response = await fetch("http://localhost:5033/api/View/ViewMyCart", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error);
    }
    const data = await response.json();
    console.log(data);
  
    CreateElement(data);
    CreateCheckout(data);
  } catch (error) {
    spinner.style.visibility = "hidden";
    var Container = document.getElementById("cart-cont");
    Container.innerHTML = `<p class="empty-cart">No Item available</p>`;
    // console.error("An error occurred while fetching data:", error.message);
    // console.log("kkkkkkkkkkkkkkkkkkkkkkkkkkkkooooooooooooooooooooooooooooo");
  }
}
function CreateCheckout(data) {
  var totalItem = data.length;
  var deliverycharge = 100;
  var totalamount = 0;
  data.forEach((element) => {
    totalamount += element.quantity * element.cost;
  });
  if (totalamount >= 500) {
    deliverycharge = 0;
  }
  var totalItemCont = document.getElementById("total-item");
  var deliveryCont = document.getElementById("delivery-charge");
  var totalAmountCont = document.getElementById("total-amount");
  var estimateAmontCont = document.getElementById("final-total");
  totalAmountCont.innerHTML = totalamount;
  totalItemCont.innerHTML = totalItem;
  deliveryCont.innerHTML = deliverycharge;
  estimateAmontCont.innerHTML = totalamount + deliverycharge;
  var freetext = document.getElementById("free-txt");
  var freebar = document.getElementById("free-bar");
  if (totalamount < 500) {
    freetext.innerHTML = `Add ${500 - totalamount} to get free delivery`;
    var percent = (totalamount / 500) * 100;

    freebar.innerHTML = percent;
    freebar.style.width = `${percent}`;
  } else {
    freetext.innerHTML = "You are eligible for Free delivery";
    freebar.innerHTML = "100%";
    freebar.style.width = "100%";
  }
}

function CreateElement(data) {
  var content = "";
  var Container = document.getElementById("cart-cont");
  data.forEach((element) => {
    content += `   <div class="card dark">
         <div class="img-cont">
            <img
              src="data:image/jpeg;base64,${element.image}"
              class="card-img-top"
              alt="..."
            />
            </div>
            <div class="card-body">
              <span class="delete-btn" value=${element.cartId} onclick="Delete(event)">&times;</span>
              <div class="text-section">
                <h5 class="card-title fw-bold">${element.medicineName}</h5>
                <p class="card-text company">${element.brand}</p>
                <div class="card-cont">
                  <p class="card-text">Weight:${element.weight}</p>
                  <p class="card-text">Pack of ${element.itemPerPack}-item</p>
                </div>
              </div>
              <div class="cta-section">
                <div class="price">$${element.cost}</div>
                <div class="quantity-field">
                  <button
                    class="value-button decrease-button"
                    value=${element.cartId}
                    onclick="decreaseValue(this)"
                    title="Azalt"
                  >
                    -
                  </button>
                  <div class="number">${element.quantity}</div>
                  <button
                  value=${element.cartId}
                    class="value-button increase-button"
                    onclick="increaseValue(this, 10)"
                    title="Arrtır"
                  >
                    +
                  </button>
                </div>
              </div>
            </div>
          </div>`;
  });
  Container.innerHTML = content;
}

async function increaseValue(button, limit) {
  const numberInput = button.parentElement.querySelector(".number");
  var spinner = document.querySelector(".custom-spinner");

  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (limit && value >= limit) return;
  spinner.style.visibility = "visible";
  try {
    params = {
      medicineId: button.value,
      status: "Increase",
      quantity: 1,
      cartId: button.value,
    };
    console.log(params);
    const response = await fetch("http://localhost:5033/api/Cart/UpdateCart", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
      body: JSON.stringify(params),
    });
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }
    numberInput.innerHTML = value + 1;
    await Initialize();
    const data = await response.json();
    console.log("Data posted successfully:", data);
  } catch (error) {
    spinner.style.visibility = "hidden";
    Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989);",
      },
    }).showToast();
    console.error("An error occurred while posting data:", error.message);
  }
}

async function decreaseValue(button) {
  var spinner = document.querySelector(".custom-spinner");
  spinner.style.visibility = "visible";
  const numberInput = button.parentElement.querySelector(".number");
  var value = parseInt(numberInput.innerHTML, 10);
  if (isNaN(value)) value = 0;
  if (value < 1) return;
  try {
    params = {
      medicineId: button.value,
      status: "Decrease",
      quantity: 1,
      cartId: button.value,
    };

    const response = await fetch("http://localhost:5033/api/Cart/UpdateCart", {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
      body: JSON.stringify(params),
    });
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }

    numberInput.innerHTML = value - 1;

    const data = await response.json();
    await Initialize();
    console.log("Data posted successfully:", data);
  } catch (error) {
    Toastify({
      text: error.message,
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989);",
      },
    }).showToast();

    console.error("An error occurred while posting data:", error.message);
  }
}
async function Delete(event) {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const value = event.target.getAttribute("value");
    console.log(value);
    spinner.style.visibility = "visible";
    const response = await fetch(
      `http://localhost:5033/api/Cart/RemoveFromCart?CartId=${value}`,
      {
        method: "DELETE",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
      }
    );
    spinner.style.visibility = "hidden";
    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }
    var result = await response.json();
    console.log(result);
    Toastify({
      text: "Item Removed Successfull",
      duration: 3000,
      gravity: "top",
      position: "right",
      style: {
        background: "rgba(241, 73, 47, 0.989);",
      },
    }).showToast();
    await Initialize();
  } catch (error) {
    spinner.style.visibility = "hidden";
  }
}

async function Checkout() {
  try {
    const response = await fetch("http://localhost:5033/api/Cart/Checkout", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });

    if (!response.ok) {
      const errorResponse = await response.json();
      throw new Error(
        `Error ${response.status}: ${
          errorResponse.message || response.statusText
        }`
      );
    }

    const data = await response.json();
    await Initialize();
    console.log("Data posted successfully:", data);
  } catch (error) {
    console.error("An error occurred while posting data:", error.message);
  }
}
