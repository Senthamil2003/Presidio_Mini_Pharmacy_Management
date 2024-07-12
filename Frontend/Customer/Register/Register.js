const form = document.getElementById("registerForm");
const inputs = form.querySelectorAll("input");

const validations = {
  name: {
    regex: /^[a-zA-Z\s]{2,30}$/,
    error:
      "Name should be 2-30 characters long and contain only letters and spaces.",
  },
  email: {
    regex: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    error: "Please enter a valid email address.",
  },
  password: {
    regex: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/,
    error:
      "Password must be at least 8 characters long and include uppercase, lowercase, and numbers.",
  },
  phone: {
    regex: /^\d{10}$/,
    error: "Mobile number should be 10 digits long.",
  },
  address: {
    regex: /.+/,
    error: "Please enter your address.",
  },
};

function validateInput(input) {
  const errorElement = document.getElementById(`${input.id}Error`);
  let isValid;

  if (input.id === "confirmPassword") {
    isValid = input.value === document.getElementById("password").value;
    if (!isValid) {
      errorElement.textContent = "Passwords do not match.";
    }
  } else {
    isValid = validations[input.id].regex.test(input.value);
    if (!isValid) {
      errorElement.textContent = validations[input.id].error;
    }
  }

  if (isValid) {
    input.classList.remove("invalid");
    input.classList.add("valid");
    errorElement.style.display = "none";
  } else {
    input.classList.remove("valid");
    input.classList.add("invalid");
    errorElement.style.display = "block";
  }

  return isValid;
}

inputs.forEach((input) => {
  input.addEventListener("blur", function () {
    validateInput(this);
  });
});

form.addEventListener("submit", async function (e) {
  e.preventDefault();
  let isValid = true;
  const formData = {
    name: "",
    email: "",
    password: "",
    phone: "",
    address: "",
    role: "User",
  };

  inputs.forEach((input) => {
    if (!validateInput(input)) {
      isValid = false;
    }
    if (input.id != "confirmPassword") {
      formData[input.id] = input.value;
    }
  });

  if (isValid) {
    console.log("Form data:", formData);
    try {
      const response = await fetch("http://localhost:5033/api/Auth/Register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(formData),
      });

      if (!response.ok) {
        const errorResponse = await response.json();
        throw new Error(` ${errorResponse.message || response.statusText}`);
      }

      const data = await response.json();
      console.log("Data posted successfully:", data);
      Toastify({
        text: "User Registered Successfully",
        duration: 3000,
        gravity: "top",
        position: "right",
        style: {
          background: "rgba(12, 235, 49, 0.989)",
        },
      }).showToast();
      setTimeout(() => {
        window.location.href="../Login/Login.html";
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
  } else {
    console.log("Form has errors. Please correct them.");
  }
});
