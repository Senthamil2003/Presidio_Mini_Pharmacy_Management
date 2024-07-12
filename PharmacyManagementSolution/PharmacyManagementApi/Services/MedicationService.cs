using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Logging;
using PharmacyManagementApi.Repositories.General_Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        private readonly CustomerMedicationRepository _customerMedicationRepo;
        private readonly CustomerCartRepository _customerCartRepo;
        private readonly IRepository<int, Cart> _cartRepo;

        public MedicationService(ITransactionService transactionService,
            CustomerJoinedRepository customerJoinedRepo,
            IRepository<int, Medication> medicationRepo,
            IRepository<int, MedicationItem> medicationItemRepo,
            IRepository<int, Medicine> medicineRepo,
            IRepository<int, Order> orderRepo,
            IRepository<int, OrderDetail> orderDetailRepo,
            IRepository<int, Cart> cartrepo,
            CustomerMedicationRepository customerMedicationRepo,
            CustomerCartRepository customerCartRepo,
            ILogger<MedicationService> logger)
        {
            _transactionService = transactionService;
            _medicationRepo = medicationRepo;
            _medicationItemRepo = medicationItemRepo;
            _medicineRepo = medicineRepo;
            _customerJoinedRepo = customerJoinedRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _cartRepo = cartrepo;
            _customerMedicationRepo= customerMedicationRepo;
            _customerCartRepo= customerCartRepo;
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
                        CreatedDate = DateTime.Now,
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
        public async Task<SuccessMedicationDTO> AddMedicationItem(AddMedicationItemDTO updateMedication)
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
                    Medication medication = await _medicationRepo.Get(updateMedication.MedicationId);
                    if (updateMedication.CustomerId != medication.CustomerId)
                    {
                        throw new NoMedicationFoundException("No medication found for the customer");
                    }
                    MedicationItem medicationItem = await _medicationItemRepo.Get(updateMedication.MedicationItemId);
                    if(medication.MedicationId!= medicationItem.MedicationId)
                    {
                        throw new  NoMedicationItemFoundException("There is no medication item found for given medication");
                    }
                    int itemQuantity=medicationItem.Quantity;   

                    if (updateMedication.Status == "Increase")
                    {
                        itemQuantity += updateMedication.Quantity;
                    }
                    else if (updateMedication.Status == "Decrease")
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
                    medicationItem.Quantity=itemQuantity;
                    await _medicationItemRepo.Update(medicationItem);

                    await _transactionService.CommitTransactionAsync();

                    SuccessMedicationDTO success = new SuccessMedicationDTO()
                    {
                        Code = 200,
                        Message = "MedicationItem updated successfully",
                        MedicationId=medication.MedicationId

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
        public async Task<SuccessRemoveDTO> RemoveMedication(int customerId, int MedicationId)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {

                    Medication medication = await _medicationRepo.Get(MedicationId);
                    if (medication.CustomerId != customerId)
                    {
                        throw new NoMedicationFoundException("No Medication for Customer found with id");
                    }
                    List<MedicationItem> medicationItems = medication.MedicationItems.ToList();
                    foreach (var item in medicationItems)
                    {
                        await _medicationItemRepo.Delete(item.MedicationItemId);
                    }
                    await _medicationRepo.Delete(MedicationId);
                    SuccessRemoveDTO successRemove = new SuccessRemoveDTO()
                    {
                        code = 200,
                        message = "Meidcation Removed successfully"
                    };
                    await _transactionService.CommitTransactionAsync();
                    return successRemove;

                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;

                }
            }

        }
        public async Task<SuccessRemoveDTO> RemoveMedicationItem(int medicationId, int customerId, int MedicationItemId)
        {
            try
            {
                Medication medication = await _medicationRepo.Get(medicationId);
                if (medication.CustomerId != customerId)
                {
                    throw new NoMedicationItemFoundException("The customer has no such medication Item");
                }
                await _medicationItemRepo.Delete(MedicationItemId);
                SuccessRemoveDTO successRemoveDTO = new SuccessRemoveDTO()
                {
                    code = 200,
                    message = "Medication removed Sucessfull"
                };
                return successRemoveDTO;

            }
            catch
            {
                throw;
            }


        }
        public async Task<bool> CheckExistingCart(int userId, int medicineId, int quantity,int currentQuantity,string medicineName)
        {
            var existingCart = (await _customerCartRepo.Get(userId)).Carts.ToList();
            var cartItem = existingCart.FirstOrDefault(c => c.MedicineId == medicineId);

            if (cartItem != null)
            {
                if(currentQuantity < quantity+cartItem.Quantity) {

                    throw new OutOfStockException($"The expected stock is not available for {medicineName}");

                }
                cartItem.Quantity += quantity;
                await _cartRepo.Update(cartItem);
                return true;
            }

            return false;
        }

        public async Task<SuccessCheckoutDTO> BuyMedication(int userId, int medicationId)
        {
            _logger.LogInformation("Performing medication checkout for customer {UserId} and medication {MedicationId}", userId, medicationId);
            using var transaction = await _transactionService.BeginTransactionAsync();
            try
            {
                var medication = await _medicationRepo.Get(medicationId);
                if (medication.CustomerId != userId)
                {
                    throw new NoMedicationFoundException("The customer has no such medication");
                }

                var medicationItems = medication.MedicationItems.ToList();

                foreach (var item in medicationItems)
                {
                    var medicine = await _medicineRepo.Get(item.MedicineId);

                    if (medicine == null)
                    {
                        throw new Exception($"Medicine with ID {item.MedicineId} not found");
                    }

                    if (medicine.CurrentQuantity < item.Quantity)
                    {
                        throw new OutOfStockException($"The expected stock is not available for {medicine.MedicineName}");
                    }

                    if (!(await CheckExistingCart(userId, item.MedicineId, item.Quantity,medicine.CurrentQuantity,medicine.MedicineName)))
                    {
                        var cart = new Cart
                        {
                            CustomerId = userId,
                            Date = DateTime.Now,
                            Cost = medicine.SellingPrice,
                            MedicineId = item.MedicineId,
                            Quantity = item.Quantity,
                            TotalCost = item.Quantity * medicine.SellingPrice,
                        };
                        await _cartRepo.Add(cart);
                    }
                }

                await _transactionService.CommitTransactionAsync();

                return new SuccessCheckoutDTO
                {
                    Code = 200,
                    Message = "Medication Order Completed"
                };
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