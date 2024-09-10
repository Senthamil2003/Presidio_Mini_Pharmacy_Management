const form = document.getElementById("loginForm");
const email = document.getElementById("email");
const password = document.getElementById("password");
const emailError = document.getElementById("emailError");
const passwordError = document.getElementById("passwordError");

function validateEmail(email) {
  const re =
    /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return re.test(String(email).toLowerCase());
}

function validatePassword(password) {
  return password.length >= 8;
}

function showError(input, message) {
  const error = input.nextElementSibling;
  error.textContent = message;
  error.style.display = "block";
  input.style.borderColor = "#ff0000";
}

function showSuccess(input) {
  const error = input.nextElementSibling;
  error.style.display = "none";
  input.style.borderColor = "#4CAF50";
}

email.addEventListener("blur", function () {
  if (!validateEmail(email.value.trim())) {
    showError(email, "Please enter a valid email address");
  } else {
    showSuccess(email);
  }
});

password.addEventListener("blur", function () {
  if (!validatePassword(password.value.trim())) {
    showError(password, "Password must be at least 8 characters long");
  } else {
    showSuccess(password);
  }
});

form.addEventListener("submit", async function (e) {
  e.preventDefault();

  let isValid = true;

  if (!validateEmail(email.value.trim())) {
    showError(email, "Please enter a valid email address");
    isValid = false;
  } else {
    showSuccess(email);
  }

  if (!validatePassword(password.value.trim())) {
    showError(password, "Password must be at least 8 characters long");
    isValid = false;
  } else {
    showSuccess(password);
  }

  if (isValid) {
    console.log("Email:", email.value);
    console.log("Password:", password.value);
    var params = {
      email: email.value,
      password: password.value,
    };
    try {
      const response = await fetch("http://localhost:5033/api/Auth/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(params),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        throw new Error(` ${errorResponse.message || response.statusText}`);
      }

      const data = await response.json();
      console.log("Data posted successfully:", data);
      Toastify({
        text: "Login Successfull...",
        duration: 3000,
        gravity: "top",
        position: "right",
        style: {
          background: "rgba(12, 235, 49, 0.989)",
        },
      }).showToast();
      localStorage.setItem("token", data.accessToken);
      setTimeout(() => {
        if (data.role == "User") {
          window.location.href = "../Home/Home.html";
        } else if (data.role == "Admin") {
          window.location.href = "../../Admin/Report/PurchaseReport.html";
        }
      }, 1000);
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

      console.error("An error occurred while posting data:", error.message);
    }
  }
});
