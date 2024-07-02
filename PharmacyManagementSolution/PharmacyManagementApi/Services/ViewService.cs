using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using Microsoft.Extensions.Logging;
using PharmacyManagementApi.Repositories.General_Repositories;
using Microsoft.EntityFrameworkCore;

namespace PharmacyManagementApi.Services
{
    public class ViewService : IViewService
    {
        private readonly StockJoinedRepository _stockRepo;
        private readonly CustomerJoinedRepository _customer;
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly IRepository<int, Brand> _brandRepo;
        private readonly IRepository<int, Category> _categoryRepo;
        private readonly ILogger<ViewService> _logger;

        public ViewService(
            StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            ILogger<ViewService> logger,
            IRepository<int,Medicine> medicinerepo,
            IRepository<int ,Category> categoryrepo,
            IRepository<int,Brand> brandrepo
            )
            
        {
            _stockRepo = stockJoinedRepo;
            _customer = customerJoinRepo;
            _logger = logger;
            _medicineRepo=medicinerepo;
            _brandRepo=brandrepo;
            _categoryRepo=categoryrepo;
        }

        /// <summary>
        /// Shows all available products with their details.
        /// </summary>
        /// <returns>An array of stock response DTOs containing product information.</returns>
        public async Task<StockResponseDTO[]> ShowAllProduct(string searchContent)
        {
            _logger.LogInformation("Fetching all product details");

            try
            {
            
              List<Medicine> medicines = (await _medicineRepo.Get()) 
                    .Where(data => data.MedicineName.IndexOf(searchContent, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(m => m.FeedbackCount > 0 ? (double)m.FeedbackSum / m.FeedbackCount : 0)
                    .ToList();



                StockResponseDTO[] responseDTO = new StockResponseDTO[medicines.Count()];
                int ct = 0;
                foreach (var item in medicines)
                {
                    StockResponseDTO response = new StockResponseDTO()
                    {
                        MedicineId = item.MedicineId,
                        MedicineName = item.MedicineName,
                        Category = item.Category.CategoryName,
                        Amount = item.SellingPrice,
                        AvailableQuantity = item.CurrentQuantity,
                        FeedbackCount=item.FeedbackCount,
                        FeedbackSum=item.FeedbackSum,
                        ItemPerPack = item.ItemPerPack,
                        Weight=item.Weight,
                        BrandName=(await _brandRepo.Get(item.BrandId)).BrandName,
                        Image = item.Image != null ? Convert.ToBase64String(item.Image) : null
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
        public async Task<List<MyMedicationDTO>> ViewMyMedications(int customerId)
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
                List<MyMedicationDTO> result = new List<MyMedicationDTO>();

                foreach (var item in medications)
                {
                    MyMedicationDTO addMedicationDTO = new MyMedicationDTO()
                    {
                        MedicationId=item.MedicationId,
                        CustomerId = customerId,
                        MedicationName = item.MedicationName,
                        Description=item.Description,
                        CreatedDate = item.CreatedDate.Date,
                        TotalCount=item.MedicationItems.Count

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

        /// <summary>
        /// Gets all cart items for a given customer.
        /// </summary>
        /// <param name="userId">The customer ID.</param>
        /// <returns>A list of Cart items for the customer</returns>
        public async Task<List<MyCartDTO>> ViewMyCart(int userId)
        {
            try
            {
                Customer customer =await _customer.Get(userId);
                List<Cart> carts = customer.Carts.ToList();
                List<MyCartDTO> result = new List<MyCartDTO>();
                if(carts.Count == 0) {
                    throw new NoCartFoundException("No cart items found for customer "+ userId);
                }
                foreach(var cart in carts)
                {
                    MyCartDTO cartDTO = new MyCartDTO()
                    {

                        CartId = cart.CartId,
                        Cost = cart.Cost,
                        MedicineId=cart.Medicine.MedicineId,
                        MedicineName = cart.Medicine.MedicineName,
                        Quantity = cart.Quantity,
                        Image= cart.Medicine.Image != null ? Convert.ToBase64String(cart.Medicine.Image) : null,
                        Brand= (await _brandRepo.Get(cart.Medicine.BrandId)).BrandName,
                        ItemPerPack=cart.Medicine.ItemPerPack,
                        Weight=cart.Medicine.Weight,
                        


                    };
                    result.Add(cartDTO);
                   

                }
                return result;

            }
            catch
            {
                throw;
            }
        }
        public async Task<List<BestSellerDTO>> GetBestSeller()
        {
            try
            {
               List<Medicine> medicines= (await _medicineRepo.Get()).
                    Where(e => e.status == 1).
                    OrderByDescending(e => e.TotalNumberOfPurchase).
                    ThenByDescending(e =>e.FeedbackSum /e.FeedbackCount)
                    .Take(10)
                    .ToList();
                List<BestSellerDTO> bestSellers = new List<BestSellerDTO>();
                foreach (var item in medicines)
                {
                    BestSellerDTO bestSell = new BestSellerDTO()
                    {
                        Brand = (await _brandRepo.Get(item.BrandId)).BrandName,
                        MedicineName = item.MedicineName,
                        Price = item.SellingPrice,
                        Rating = item.FeedbackCount!=0? (item.FeedbackSum / item.FeedbackCount):0,
                        Image = item.Image != null ? Convert.ToBase64String(item.Image) : null,

                    };
                    bestSellers.Add(bestSell);
                }
                return bestSellers;
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<BestCategoryDTO>> GetBestCategory()
        {
            try
            {
                var category = (await _medicineRepo.Get()).
                     Where(e => e.status == 1).
                     OrderByDescending(e => e.TotalNumberOfPurchase).
                     ThenByDescending(e => e.FeedbackSum / e.FeedbackCount)
                     .GroupBy(e => e.CategoryId)
                     .Take(6);
                    
                List<BestCategoryDTO> bestCategorys = new List<BestCategoryDTO>();
                foreach (var item in category)
                {
                    Category categoryitem = (await _categoryRepo.Get(item.Key));
                    BestCategoryDTO bestcategory = new BestCategoryDTO()
                    {
                        CategoryId=item.Key,
                        CategoryName=categoryitem.CategoryName,
                        Image = categoryitem.Image != null ? Convert.ToBase64String(categoryitem.Image) : null,

                    };
                    bestCategorys.Add(bestcategory);
                }
                return bestCategorys;
            }
            catch
            {
                throw;
            }
        }
        public async Task<ProductDTO> GetMedicine(int Id)
        {
            try
            {
                Medicine medicine = await _medicineRepo.Get(Id);
                ProductDTO medicineDetail = new ProductDTO()
                {
                    MedicineId=medicine.MedicineId,
                    CategoryName = (await _categoryRepo.Get(medicine.CategoryId)).CategoryName,
                    MedicineName = medicine.MedicineName,
                    ImageBase64 = medicine.Image != null ? Convert.ToBase64String(medicine.Image) : null,
                    Brand = medicine.Brand.BrandName,
                    Description = medicine.Description,
                    ItemPerPack=medicine.ItemPerPack,
                    Weight=medicine.Weight,
                    SellingPrice = medicine.SellingPrice,
                    FeedbackCount=medicine.FeedbackCount,
                    FeedbackSum=medicine.FeedbackSum,

                };
                return medicineDetail;

            }
            catch
            {
                throw;
            }
        }

    }
}