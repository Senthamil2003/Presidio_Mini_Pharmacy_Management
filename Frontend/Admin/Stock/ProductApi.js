import { GetData, PostData } from "../FetchApi/Api.js";
async function GetAllMedicine() {
    var table=document.getElementById("product-table");
    var content=""
    var Medicine = await GetData(
      "http://localhost:5033/api/Admin/GetAllMedicine",
      {}
    );
  
    Medicine.forEach((element,i) => {
      content += ` <tr>
                <td>1</td>
                <td>${i+1}aracetamol</td>
                <td>${element.MedicineName}</td>
                <td>${element.CategoryName}</td>
                <td>${element.Brand}</td>
                <td><div class="approved">Active</div></td>
                <td><button class="btn btn-outline-success">View</button></td>
              </tr>`;
    });
    
  }
  