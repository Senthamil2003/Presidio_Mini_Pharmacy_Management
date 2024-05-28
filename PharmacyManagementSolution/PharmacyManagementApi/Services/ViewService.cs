using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;

namespace PharmacyManagementApi.Services
{
    public class ViewService:IViewService
    {
        private readonly StockJoinedRepository _stockRepo;
        private readonly CustomerJoinedRepository _customer;

        public ViewService (
            StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo
            ) {
            _stockRepo=stockJoinedRepo;
            _customer=customerJoinRepo;

        }
        public async Task<StockResponseDTO[]> ShowAllProduct()
        {
            try
            {
                var result = (await _stockRepo.Get()).GroupBy(s => s.MedicineId).Select(item => new
                {
                    MedicineId = item.Key,
                    TotalQuantity = item.Sum(i => i.Quantity),
                    SellingPrice = item.Max(i => i.SellingPrice),
                    MedicineName = item.First().Medicine.MedicineName,
                    CategoryName = item.First().Medicine.Category.CategoryName,
                    Rating =item.First().Medicine.FeedbackSum/ item.First().Medicine.FeedbackCount
                }).OrderByDescending(e=>e.Rating);
                StockResponseDTO[] responseDTO = new StockResponseDTO[result.Count()];
                int ct = 0;
                foreach (var item in result)
                {
                    StockResponseDTO response = new StockResponseDTO()
                    {
                        MedicineId = item.MedicineId,
                        MedicineName = item.MedicineName,
                        Category = item.CategoryName,
                        Amount = item.SellingPrice,
                        AvailableQuantity = item.TotalQuantity

                    };
                    responseDTO[ct] = response;
                    ct++;

                }
                return responseDTO;

            }
            catch
            {
                throw;

            }
        }
        public async Task<List<MyOrderDTO>> GetAllOrders(int userId)
        {
            try
            {
                Customer customer = await _customer.Get(userId);
                List<MyOrderDTO> myOrders = new List<MyOrderDTO>();
                List<Order> orders = customer.Orders.ToList();
                if (orders.Count == 0)
                {
                    throw new NoOrderFoundException("There is no order for the customer");
                }
                foreach (var item in orders)
                {
                    List<OrderDetail> orderDetails = item.OrderDetails.ToList();
                    foreach (var orderDetail in orderDetails)
                    {
                        List<DeliveryDetail> deliveryDetails = orderDetail.DeliveryDetails.ToList();
                        foreach (var deliveryDetail in deliveryDetails)
                        {
                            MyOrderDTO myOrder = new MyOrderDTO()
                            {
                                OrderDetailId = orderDetail.OrderDetailId,
                                ExpiryDate = deliveryDetail.ExpiryDate,
                                Quantity = deliveryDetail.Quantity,
                                MedicineName = deliveryDetail.Medicine.MedicineName
                            };
                            myOrders.Add(myOrder);

                        }


                    }

                }
                return myOrders;

            }
            catch
            {
                throw;
            }
        }
    }
}
