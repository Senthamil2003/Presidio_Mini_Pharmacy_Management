using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyManagementApi.Services
{
    public class ViewService : IViewService
    {
        private readonly StockJoinedRepository _stockRepo;
        private readonly CustomerJoinedRepository _customer;
        private readonly ILogger<ViewService> _logger;

        public ViewService(
            StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            ILogger<ViewService> logger)
        {
            _stockRepo = stockJoinedRepo;
            _customer = customerJoinRepo;
            _logger = logger;
        }

        /// <summary>
        /// Shows all available products with their details.
        /// </summary>
        /// <returns>An array of stock response DTOs containing product information.</returns>
        public async Task<StockResponseDTO[]> ShowAllProduct()
        {
            _logger.LogInformation("Fetching all product details");

            try
            {
                var result = (await _stockRepo.Get()).GroupBy(s => s.MedicineId).Select(item => new
                {
                    MedicineId = item.Key,
                    TotalQuantity = item.Sum(i => i.Quantity),
                    SellingPrice = item.Max(i => i.SellingPrice),
                    MedicineName = item.First().Medicine.MedicineName,
                    CategoryName = item.First().Medicine.Category.CategoryName,
                    Rating = item.First().Medicine.FeedbackSum / item.First().Medicine.FeedbackCount
                }).OrderByDescending(e => e.Rating);

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

                _logger.LogInformation("Product details fetched successfully");
                return responseDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching product details");
                throw;
            }
        }

        /// <summary>
        /// Views all the medications for a given customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A list of medication DTOs containing the customer's medication information.</returns>
        public async Task<List<AddMedicationDTO>> ViewMyMedications(int customerId)
        {
            _logger.LogInformation("Fetching medications for customer {CustomerId}", customerId);

            try
            {
                Customer customer = await _customer.Get(customerId);
                if (customer.Medications.Count == 0)
                {
                    _logger.LogWarning("No medications found for customer {CustomerId}", customerId);
                    throw new NoMedicationFoundException("Customer have no medication");
                }

                List<Medication> medications = customer.Medications.ToList();
                List<AddMedicationDTO> result = new List<AddMedicationDTO>();
                foreach (var item in medications)
                {
                    AddMedicationDTO addMedicationDTO = new AddMedicationDTO()
                    {
                        CustomerId = customerId,
                        MedicationName = item.MedicationName
                    };
                    MedicationItemDTO[] medicationItemsDTO = new MedicationItemDTO[item.MedicationItems.Count];
                    int ct = 0;
                    foreach (MedicationItem item2 in item.MedicationItems)
                    {
                        medicationItemsDTO[ct++] = new MedicationItemDTO()
                        {
                            MedicineId = item2.MedicineId,
                            Quantity = item2.Quantity,
                        };
                    }
                    addMedicationDTO.medicationItems = medicationItemsDTO;
                    result.Add(addMedicationDTO);
                }

                _logger.LogInformation("{Count} medications found for customer {CustomerId}", result.Count, customerId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching medications for customer {CustomerId}", customerId);
                throw;
            }
        }

        /// <summary>
        /// Gets all orders for a given customer.
        /// </summary>
        /// <param name="userId">The customer ID.</param>
        /// <returns>A list of order DTOs containing the customer's order information.</returns>
        public async Task<List<MyOrderDTO>> GetAllOrders(int userId)
        {
            _logger.LogInformation("Fetching orders for customer {UserId}", userId);

            try
            {
                Customer customer = await _customer.Get(userId);
                List<MyOrderDTO> myOrders = new List<MyOrderDTO>();
                List<Order> orders = customer.Orders.ToList();

                if (orders.Count == 0)
                {
                    _logger.LogWarning("No orders found for customer {UserId}", userId);
                    throw new NoOrderFoundException("There is no order for the customer");
                }

                foreach (var order in orders)
                {
                    List<OrderDetail> orderDetails = order.OrderDetails.ToList();

                    foreach (var orderDetail in orderDetails)
                    {
                        List<DeliveryDetailDTO> deliveryDetailDTOs = new List<DeliveryDetailDTO>();

                        foreach (var deliveryDetail in orderDetail.DeliveryDetails)
                        {
                            DeliveryDetailDTO deliveryDetailDTO = new DeliveryDetailDTO()
                            {
                                ExpiryDate = deliveryDetail.ExpiryDate,
                                Quantity = deliveryDetail.Quantity,
                                MedicineName = deliveryDetail.Medicine.MedicineName,
                                DeliveryDate = deliveryDetail.DeliveryDate
                            };
                            deliveryDetailDTOs.Add(deliveryDetailDTO);
                        }

                        MyOrderDTO myOrder = new MyOrderDTO()
                        {
                            MedicineName = orderDetail.Medicine.MedicineName,
                            OrderDetailId = orderDetail.OrderDetailId,
                            DeliveryDetails = deliveryDetailDTOs,
                        };

                        myOrders.Add(myOrder);
                    }
                }

                _logger.LogInformation("{Count} orders found for customer {UserId}", myOrders.Count, userId);
                return myOrders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for customer {UserId}", userId);
                throw;
            }
        }
    }
}