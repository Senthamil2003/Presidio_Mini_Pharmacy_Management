import { GetData, PostData } from "../FetchApi/Api.js";
async function GetAllMedicine() {
  var Medicine = await GetData(
    "http://localhost:5033/api/Admin/GetAllMedicine",
    {}
  );
  console.log(Medicine);
  Medicine.forEach((element) => {});
}
await GetAllMedicine();
