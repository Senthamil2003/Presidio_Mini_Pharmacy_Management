var token = localStorage.getItem("token") || null;
document.onload = Validate();

async function Validate() {
  try {
    var spinner = document.querySelector(".custom-spinner");
    spinner.style.visibility = "visible";
    const response = await fetch("http://localhost:5033/api/Auth/validate", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + token,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error);
    }
    const data = await response.json();
    console.log(data);
    await MyOrder();
  } catch (error) {
    console.log(error.message);
    window.location.href = "../Login/Login.html";
  }
}
document.getElementById("logout").addEventListener("click", () => {
  localStorage.removeItem("token");
  location.reload();
});

function formatDate(dateString) {
  var date = new Date(dateString);
  var day = date.getDate().toString().padStart(2, "0");
  var month = (date.getMonth() + 1).toString().padStart(2, "0");
  var year = date.getFullYear();
  return `${day}/${month}/${year}`;
}

async function MyOrder() {
  try {
    var spinner = document.querySelector(".custom-spinner");

    const response = await fetch(
      "http://localhost:5033/api/View/ViewMyOrders",
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + token,
        },
      }
    );
    if (!response.ok) {
      const error = await response.json();
      console.log(error);
      throw new Error(error);
    }
    var data = await response.json();
    console.log(data);
    CreateElement(data);
  } catch (error) {
    document.querySelector(".no-item-found").style.visibility = "visible";
    document.querySelector("#order-cont").style.display = "none";
    console.log(error);
  } finally {
    spinner.style.visibility = "hidden";
  }
}

function CreateElement(data) {
  const container = document.getElementById("order-cont");
  const content = data
    .map(
      (element, i) => `
    <div class="accordion accordion-flush" id="accordionFlushExample">
      <div class="accordion-item">
        <div class="custom-accordian-data">
          <div class="card dark">
            <img
              src="data:image/jpeg;base64,${element.image}" 
              class="card-img-top"
              alt="..."
            />
            <div class="card-body">
              <div class="text-section">
                <h5 class="card-title fw-bold">${element.medicineName}</h5>
                <p class="card-text company">${element.brandName}</p>
                <div class="card-cont">
                  <h5 class="card-title fw-bold">Quantity</h5>
                  <p class="card-text company">Pack of ${element.quantity}</p>
                </div>
              </div>
              <div class="text-section">
                <h5 class="card-title fw-bold">Order on</h5>
                <p class="card-text company">${formatDate(
                  element.orderDate
                )}</p>
                <div class="card-cont">
                  <h5 class="card-title fw-bold">Order Id</h5>
                  <p class="card-text company">${element.orderDetailId}</p>
                </div>
              </div>
              <div class="cta-section">
                <div class="price">$${element.cost}</div>
                ${
                  element.status
                    ? `<div class="cta-section">
                   
                    <button
                      class="accordion-button collapsed success"
                      type="button"
                      data-bs-toggle="collapse"
                      data-bs-target="#flush-collapse-${i}"
                      aria-expanded="false"
                      aria-controls="flush-collapseOne"
                    >
                      Delivered
                    </button>
                  </div>`
                    : `<button class="btn btn-warning" disabled>Not Delivered</button>`
                }
              </div>
            </div>
          </div>
          <div
            id="flush-collapse-${i}"
            class="accordion-collapse collapse"
            aria-labelledby="flush-headingOne"
            data-bs-parent="#accordionFlushExample"
          >
            <div class="accordion-body">
              <hr />
              <table class="table">
                <thead>
                  <tr>
                    <th>#</th>
                    <th scope="col">OrderId</th>
                    <th scope="col">Medicine Name</th>
                    <th scope="col">Quantity</th>
                    <th scope="col">Expiry date</th>
                    <th scope="col">Delivery date</th>
                  </tr>
                </thead>
                <tbody>
                ${element.deliveryDetails
                  .map(
                    (item, ct) => `
                  <tr>
                    <td>${ct + 1}</td>
                    <td>${element.orderDetailId}</td>
                    <td>${element.medicineName}</td>
                    <td>${item.quantity}</td>
                    <td>${formatDate(item.expiryDate)}</td>
                    <td>${formatDate(item.deliveryDate)}</td>
                  </tr>
                `
                  )
                  .join("")}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>`
    )
    .join("");

  container.innerHTML = content;
}
