using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Logging;

namespace PharmacyManagementApi.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly ITransactionService _transactionService;
        private readonly IRepository<int, Medication> _medicationRepo;
        private readonly IRepository<int, MedicationItem> _medicationItemRepo;
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly CustomerJoinedRepository _customerJoinedRepo;
        private readonly IRepository<int, Order> _orderRepo;
        private readonly IRepository<int, OrderDetail> _orderDetailRepo;
        private readonly ILogger<MedicationService> _logger;

        public MedicationService(ITransactionService transactionService,
            CustomerJoinedRepository customerJoinedRepo,
            IRepository<int, Medication> medicationRepo,
            IRepository<int, MedicationItem> medicationItemRepo,
            IRepository<int, Medicine> medicineRepo,
            IRepository<int, Order> orderRepo,
            IRepository<int, OrderDetail> orderDetailRepo,
            ILogger<MedicationService> logger)
        {
            _transactionService = transactionService;
            _medicationRepo = medicationRepo;
            _medicationItemRepo = medicationItemRepo;
            _medicineRepo = medicineRepo;
            _customerJoinedRepo = customerJoinedRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new medication for a customer.
        /// </summary>
        /// <param name="addMedication">The request containing the customer ID, medication name, and medication items.</param>
        /// <returns>A success DTO containing the medication ID.</returns>
        public async Task<SuccessMedicationDTO> AddMedication(AddMedicationDTO addMedication)
        {
            _logger.LogInformation("Adding medication for customer {CustomerId}", addMedication.CustomerId);

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {


                    Medication medication = new Medication()
                    {
                        CustomerId = addMedication.CustomerId,
                        MedicationName = addMedication.MedicationName,
                        Description = addMedication.MedicationDescription,
                        CreatedDate=DateTime.Now,
                    };

                    await _medicationRepo.Add(medication);
                    //HashSet<int> checkDuplicate = new HashSet<int>();
                    //var result = addMedication.medicationItems.ToList();
                    //foreach (var item in result)
                    //{
                    //    if (checkDuplicate.Contains(item.MedicineId))
                    //    {
                    //        throw new DuplicateValueException("Duplicate medicine present in the Medication");
                    //    }
                    //    Medicine medicine = await _medicineRepo.Get(item.MedicineId);

                    //    MedicationItem medicationItem = new MedicationItem()
                    //    {
                    //        MedicineId = item.MedicineId,
                    //        MedicationId = medication.MedicationId,
                    //        Quantity = item.Quantity
                    //    };
                    //    await _medicationItemRepo.Add(medicationItem);
                    //    checkDuplicate.Add(item.MedicineId);
                    //}

                    await _transactionService.CommitTransactionAsync();

                    SuccessMedicationDTO resultMedication = new SuccessMedicationDTO()
                    {
                        Code = 200,
                        Message = "Medication added successfully",
                        MedicationId = medication.MedicationId,
                    };

                    _logger.LogInformation("Medication {MedicationId} added successfully for customer {CustomerId}", medication.MedicationId, addMedication.CustomerId);
                    return resultMedication;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding medication for customer {CustomerId}", addMedication.CustomerId);
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }
        public async Task<SuccessMedicationDTO> AddMedicationItem(UpdateMedication updateMedication)
        {

            try
            {
                MedicationItem medicationItem = new MedicationItem()
                {
                    MedicationId = updateMedication.MedicationId,
                    MedicineId = updateMedication.MedicineId,
                    Quantity = updateMedication.Quantity,
                };
                await _medicationItemRepo.Add(medicationItem);
                SuccessMedicationDTO success = new SuccessMedicationDTO()
                {
                    Code = 200,
                    Message = "Medication added successfully",

                };
                return success; 

            }
            catch
            {
                throw;
            }
        }
        
        /// <summary>
        /// Updates an existing medication for a customer.
        /// </summary>
        /// <param name="updateMedication">The request containing the customer ID, medication ID, medicine ID, quantity, and update status.</param>
        /// <returns>A success DTO containing the medication ID.</returns>
        public async Task<SuccessMedicationDTO> UpdateMedication(UpdateMedication updateMedication)
        {
            _logger.LogInformation("Updating medication {MedicationId} for customer {CustomerId}", updateMedication.MedicationId, updateMedication.CustomerId);

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Medication medication=await _medicationRepo.Get(updateMedication.MedicationId);
                    int itemQuantity=medication.MedicationItems.FirstOrDefault(m=>m.MedicineId==updateMedication.MedicineId).Quantity;

                    if (updateMedication.Status == "Increase")
                    {
                        itemQuantity += updateMedication.Quantity;
                    }
                    else if(updateMedication.Status == "Decrease")
                    {
                        if (itemQuantity > 0)
                        {
                            itemQuantity -= updateMedication.Quantity;
                        }
                        else
                        {
                            throw new Exception("Not enough quantity to reduce");
                        }
                    }
                    MedicationItem updateItem = new MedicationItem()
                    {
                        MedicationId = updateMedication.MedicationId,
                        MedicineId = updateMedication.MedicineId,
                        Quantity = itemQuantity,
                        
                    };
                    await _medicationItemRepo.Update(updateItem);
                    
                    await _transactionService.CommitTransactionAsync();

                    SuccessMedicationDTO success = new SuccessMedicationDTO()
                    {
                        Code = 200,
                        Message = "Medication updated successfully",
                      
                    };

                   return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating medication {MedicationId} for customer {CustomerId}", updateMedication.MedicationId, updateMedication.CustomerId);
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Performs the checkout process for a customer's medication.
        /// </summary>
        /// <param name="userId">The customer ID.</param>
        /// <param name="medicationId">The medication ID.</param>
        /// <returns>A success DTO containing the order ID.</returns>
        public async Task<SuccessCheckoutDTO> BuyMedication(int userId, int medicationId)
        {
            _logger.LogInformation("Performing medication checkout for customer {UserId} and medication {MedicationId}", userId, medicationId);

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Medication medication = await _medicationRepo.Get(medicationId);
                    if(medication.CustomerId!= userId) {
                        throw new NoMedicationFoundException("The customer has no such medication");
                    }
                    List<MedicationItem> cart = (medication).MedicationItems.ToList();
         
                    
                    bool customerSubscribe = (await _customerJoinedRepo.Get(userId)).IsSubcribed;
                    float discount = customerSubscribe ? 10 : 0;

                    Order order = new Order()
                    {
                        CustomerId = userId,
                        Discount = discount,
                        ShipmentCost = 0,
                        TotalAmount = 0,
                        PaidAmount = 0,
                    };
                    await _orderRepo.Add(order);

                    double totalSum = 0;
                    foreach (MedicationItem item in cart)
                    {
                        var stock = await _medicineRepo.Get(item.MedicineId);
                        totalSum += stock.SellingPrice;
                        if (stock.CurrentQuantity < item.Quantity)
                        {
                            _logger.LogWarning("Expected quantity {ExpectedQuantity} is not available in stock for medicine {MedicineId}", item.Quantity, item.MedicineId);
                            throw new OutOfStockException($"Expected Quantity is not available at the moment for {stock.MedicineId}");
                        }

                        OrderDetail orderDetail = new OrderDetail()
                        {
                            MedicineId = item.MedicineId,
                            Cost = stock.SellingPrice,
                            OrderId = order.OrderId,
                            Quantity = item.Quantity
                        };
                        await _orderDetailRepo.Add(orderDetail);
                        stock.CurrentQuantity -= item.Quantity;
                        stock.TotalNumberOfPurchase += 1;
                        await _medicineRepo.Update(stock);
                    }

                    int shipmentCost = 0;
                    if (!customerSubscribe && totalSum < 500)
                    {
                        shipmentCost = 100;
                    }

                    
                    order.TotalAmount = totalSum;
                    if (discount > 0)
                    {
                        order.PaidAmount = totalSum - (totalSum * (order.Discount) / 100);
                    }
                    else
                    {
                        order.PaidAmount = totalSum+shipmentCost;
                    }

                    order.ShipmentCost = shipmentCost;
                    await _orderRepo.Update(order);

                    await _transactionService.CommitTransactionAsync();

                    SuccessCheckoutDTO checkoutDTO = new SuccessCheckoutDTO()
                    {
                        Code = 200,
                        Message = "Medication Order Completed",
                        OrderId = order.OrderId,
                    };

                    _logger.LogInformation("Medication checkout completed successfully for customer {UserId}, order ID: {OrderId}", userId, order.OrderId);
                    return checkoutDTO;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during medication checkout for customer {UserId} and medication {MedicationId}", userId, medicationId);
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }
    }
}