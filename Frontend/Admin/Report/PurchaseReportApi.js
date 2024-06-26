import { GetData, PostData } from "../FetchApi/Api.js";

function getPast30Days() {
  const toDate = new Date();

  const fromDate = new Date();
  fromDate.setDate(toDate.getDate() - 100);
  const formatDate = (date) => {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");
    const hours = String(date.getHours()).padStart(2, "0");
    const minutes = String(date.getMinutes()).padStart(2, "0");
    const seconds = String(date.getSeconds()).padStart(2, "0");
    const milliseconds = String(date.getMilliseconds()).padStart(3, "0");
    return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}.${milliseconds}Z`;
  };

  return {
    fromDate: formatDate(fromDate),
    toDate: formatDate(toDate),
  };
}

async function CreateReport() {
  var TotalContainer = document.getElementById("purchase-report");
  var { fromDate, toDate } = getPast30Days();
  const params = {
    startDate: fromDate,
    endDate: toDate,
  };
  const url = "http://localhost:5033/api/Report/PurchaseReport";
  var ReportData = await GetData(url, params);
  var content = "";
  ReportData.forEach((element, i) => {
    content += ` <tr>
                <td>${i + 1}</td>
                <td>${element.medicineId}</td>
                <td>${element.medicineName}</td>
                <td>${element.totalQuantity}</td>
                <td>${element.totalAmount}</td>
              </tr>`;
  });
  TotalContainer.innerHTML = content;
}
CreateReport();
