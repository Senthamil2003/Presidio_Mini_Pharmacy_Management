import { GetData, PostData } from "../FetchApi/Api.js";
window.onload = FetchVendor();
async function FetchVendor() {
  var Vendor = await GetData(
    "http://localhost:5033/api/Admin/GetAllVendor",
    {}
  );
  var body = document.getElementById("vendor-table");
  var content = "";
  Vendor.forEach((element, i) => {
    content += `<tr>
    <td>${i + 1}</td>
    <td>${element.vendorName}</td>
    <td>${element.address}</td>
    <td>${element.phone}</td>   
    <td>${element.mail}</td>            
    </tr>`;
  });
  content += "<tr></tr>";
  body.innerHTML = content;
}

document.getElementById("add-vendor").addEventListener("click", async () => {
  var name = document.getElementById("vendorname").value;
  var phone = document.getElementById("phone").value;
  var mail = document.getElementById("mail").value;
  var address = document.getElementById("address").value;
  console.log(name, phone, mail, address);
  var params = {
    vendorName: name,
    address: address,
    phone: phone,
    mail: mail,
  };
  var result = await PostData(
    "http://localhost:5033/api/Admin/AddVendor",
    params,
    "Post"
  );
  console.log(result);
  $("#Addvendor").modal("hide");
  await FetchVendor();
});
