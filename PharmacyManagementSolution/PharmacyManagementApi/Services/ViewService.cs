using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using Microsoft.Extensions.Logging;
using PharmacyManagementApi.Repositories.General_Repositories;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        private readonly CustomerCartRepository _customerCartRepo;
        private readonly CustomerMedicationRepository _customerMedicationRepo;
        private readonly MedicationJoinedRepository _medicationJoinedRepo;
        
        private readonly CustomerOnlyCartRepo _customerOnlyCartRepo;

        public ViewService(
            StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            CustomerCartRepository customerCart,
            CustomerMedicationRepository customerMedicationRepo,
            MedicationJoinedRepository medicationJoinedRepo,
            ILogger<ViewService> logger,
            IRepository<int,Medicine> medicinerepo,
            IRepository<int ,Category> categoryrepo,
            IRepository<int,Brand> brandrepo,
            CustomerOnlyCartRepo customerOnlyCartRepo
            )
            
        {
            _stockRepo = stockJoinedRepo;
            _customer = customerJoinRepo;
            _logger = logger;
            _medicineRepo=medicinerepo;
            _brandRepo=brandrepo;
            _categoryRepo=categoryrepo;
            _customerCartRepo = customerCart;
            _customerMedicationRepo=customerMedicationRepo;
            _medicationJoinedRepo=medicationJoinedRepo;
            _customerOnlyCartRepo=customerOnlyCartRepo;
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
                    .Where(data => data.status!=2 && data.MedicineName.IndexOf(searchContent, StringComparison.OrdinalIgnoreCase) >= 0)
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
                        Category =(await _categoryRepo.Get(item.CategoryId)).CategoryName,
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
                Customer customer = await _customerMedicationRepo.Get(customerId);
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
                if (!customer.Orders.Any())
                {
                    _logger.LogWarning("No orders found for customer {UserId}", userId);
                    throw new NoOrderFoundException("There is no order for the customer");
                }
                var orderDetails = customer.Orders.SelectMany(order => order.OrderDetails).ToList();
                var myOrders = new List<MyOrderDTO>();
                foreach (var orderDetail in orderDetails)
                {
                    var medicineName = await _medicineRepo.Get(orderDetail.MedicineId);
                    var Brandname = await _brandRepo.Get(medicineName.BrandId);
                    myOrders.Add(new MyOrderDTO
                    {
                        MedicineName = medicineName.MedicineName,
                        OrderDetailId = orderDetail.OrderDetailId,
                        BrandName=Brandname.BrandName,
                        OrderDate=orderDetail.Order.OrderDate,
                        status=orderDetail.DeliveryStatus,
                        ItemPerPack=medicineName.ItemPerPack,
                        Image= medicineName.Image != null ? Convert.ToBase64String(medicineName.Image) : null,
                        Cost =orderDetail.Cost,
                        DeliveryDetails = orderDetail.DeliveryDetails.Select(deliveryDetail => new DeliveryDetailDTO
                        {
                            DeliveryId=deliveryDetail.DeliveryId,
                            ExpiryDate = deliveryDetail.ExpiryDate,
                            Quantity = deliveryDetail.Quantity,
                            DeliveryDate = deliveryDetail.DeliveryDate
                        }).ToList()
                    });
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
    
        public async Task<List<OnlyCartItem>> ViewMyCartOnly(int userId)
        {
            try
            {
                Customer customer = await _customerOnlyCartRepo.Get(userId);
                var  carts = customer.Carts;
                List<OnlyCartItem> result = new List<OnlyCartItem>();
                if (carts.Count == 0)
                {
                    throw new NoCartFoundException("No cart items found for customer " + userId);
                }
                foreach (var cart in carts)
                {
                    OnlyCartItem onlyCartItem = new OnlyCartItem()
                    {
                        medicineId = cart.MedicineId,
                        CustomerId = cart.CustomerId
                    };
                    result.Add(onlyCartItem);


                }
                return result;

            }
            catch
            {
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
                Customer customer =await _customerCartRepo.Get(userId);
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
        public async Task<MedicationItemTotalDTO> ViewMedicationItem(int customerId,int medicationId)
        {
            try
            {
             Medication medication=  await _medicationJoinedRepo.Get(medicationId);
                if(medication.CustomerId!=customerId)
                {
                    throw new NoMedicationFoundException("NO Medication Found for the given Customer");
               }
                var items = medication.MedicationItems;
                if(items.Count==0) {
                    throw new NoMedicationFoundException("Medication item is empty");
                }
                List<MedicationItemDetailDTO> MedicationItems = new List<MedicationItemDetailDTO>(); 
                foreach( var item in items)
                {
                    MedicationItemDetailDTO itemsDTO = new MedicationItemDetailDTO()
                    {
                        amount = item.Medicine.SellingPrice,
                        BrandName = item.Medicine.Brand.BrandName,
                        MedicationItemId = item.MedicationItemId,
                        MedicineName = item.Medicine.MedicineName,
                        Image = item.Medicine.Image != null ? Convert.ToBase64String(item.Medicine.Image) : null,
                        ItemPerPack = item.Medicine.ItemPerPack,
                        Quantity = item.Quantity,
                        Weight = item.Medicine.Weight,
                    };
                    MedicationItems.Add(itemsDTO);
                }
                MedicationItemTotalDTO itemTotalDTO = new MedicationItemTotalDTO()
                {
                    MedicationName= medication.MedicationName,
                    medicationItemDetailDTOs= MedicationItems
                };
                return itemTotalDTO;

                
            }
            catch
            {
                throw;
            }

        }
        public async Task<StockResponseDTO[]> GetMedicineByCategory(string category)
        {
            try
            {
                int CategoryId= (await _categoryRepo.Get()).SingleOrDefault(e => e.CategoryName == category).CategoryId;

                List<Medicine> medicines = (await _medicineRepo.Get())
                    .Where(data => data.CategoryId==CategoryId)
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
                        Category = (await _categoryRepo.Get(item.CategoryId)).CategoryName,
                        Amount = item.SellingPrice,
                        AvailableQuantity = item.CurrentQuantity,
                        FeedbackCount = item.FeedbackCount,
                        FeedbackSum = item.FeedbackSum,
                        ItemPerPack = item.ItemPerPack,
                        Weight = item.Weight,
                        BrandName = (await _brandRepo.Get(item.BrandId)).BrandName,
                        Image = item.Image != null ? Convert.ToBase64String(item.Image) : null
                    };
                    responseDTO[ct] = response;
                    ct++;
                }

                _logger.LogInformation("Product details fetched successfully");
                return responseDTO;

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
                    .Take(8)
                    .ToList();
                List<BestSellerDTO> bestSellers = new List<BestSellerDTO>();
                foreach (var item in medicines)
                {
                    BestSellerDTO bestSell = new BestSellerDTO()
                    {
                        MedicineId=item.MedicineId,
                        FeedbackCount=item.FeedbackCount,
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
                    Brand =(await _brandRepo.Get(medicine.BrandId)).BrandName,
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