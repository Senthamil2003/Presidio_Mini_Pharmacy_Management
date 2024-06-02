using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacyUnitTest.RepositoryTest
{
    public class ReportServiceTest : BaseSetup
    {
        [Test]
        public async Task PurchaseReport()
        {
            DateTime startDate = DateTime.Parse("2024-06-01T13:07:53.893Z");
            DateTime endDate = DateTime.Parse("2024-06-30T13:07:53.893Z");

            var result =await _reportService.GetPurchasesReport(startDate,endDate);
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));

        }
        [Test]
        public async Task PurchaseReportFail()
        {
            DateTime startDate = DateTime.Parse("2024-05-01T13:07:53.893Z");
            DateTime endDate = DateTime.Parse("2024-05-30T13:07:53.893Z");

           
            var exception = Assert.ThrowsAsync<NoPurchaseDetailFoundException>(async () => await _reportService.GetPurchasesReport(startDate,endDate));
            Assert.That(exception.Message, Is.EqualTo("The purchase report is empty"));

        }
        [Test]
        public async Task OrderReport()
        {
            DateTime startDate = DateTime.Parse("2024-06-01T13:07:53.893Z");
            DateTime endDate = DateTime.Parse("2024-06-30T13:07:53.893Z");
            AddToCartDTO addToCartDTO = new AddToCartDTO()
            {
                MedicineId = 1,
                Quantity = 1,
                UserId = 1,
            };
           await _cartService.AddToCart(addToCartDTO);
           await _cartService.Checkout(1);

            var result = await _reportService.GetOrderReport(startDate, endDate);
            Assert.IsNotNull(result);
            Assert.That(result.Count(), Is.EqualTo(1));

        }
        [Test]
        public async Task OrderReportFail()
        {
            DateTime startDate = DateTime.Parse("2024-05-01T13:07:53.893Z");
            DateTime endDate = DateTime.Parse("2024-05-30T13:07:53.893Z");


            var exception = Assert.ThrowsAsync<NoOrderDetailFoundException>(async () => await _reportService.GetOrderReport(startDate, endDate));
            Assert.That(exception.Message, Is.EqualTo("The Order report is empty"));

        }
    }
}
