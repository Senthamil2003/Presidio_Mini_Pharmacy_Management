using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using System.Security.Cryptography.Xml;

namespace PharmacyManagementApi.Services
{
    public class MedicationService:IMedicationService
    {
        private readonly ITransactionService _transactionService;
        private readonly IRepository<int, Medication> _medicationRepo;
        private readonly IRepository<int, MedicationItem> _medicationItemRepo;
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly CustomerJoinedRepository _customerJoinedRepo;
        private readonly IRepository<int, Order> _orderRepo;
        private readonly IRepository<int, OrderDetail> _orderDetailRepo;

        public MedicationService(ITransactionService transactionService,
            CustomerJoinedRepository customerJoinedRepo,
            IRepository<int,Medication> medicationRepo,
            IRepository<int,MedicationItem> medicationItemRepo,
            IRepository<int ,Medicine> medicineRepo,
            IRepository<int ,Order> orderRepo,
            IRepository<int ,OrderDetail> orderDetailRepo
            
            )
        {
            _transactionService=transactionService;
            _medicationRepo=medicationRepo;
            _medicationItemRepo=medicationItemRepo;
            _medicineRepo=medicineRepo;
            _customerJoinedRepo=customerJoinedRepo;
            _orderRepo =orderRepo;
            _orderDetailRepo=orderDetailRepo;

        }

        public async Task<SuccessMedicationDTO> AddMedication(AddMedicationDTO addMedication)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Medication medication = new Medication()
                    {
                        CustomerId = addMedication.CustomerId,
                        MedicationName = addMedication.MedicationName,

                    };
                    await _medicationRepo.Add(medication);

                    var result = addMedication.medicationItems.ToList();
                    foreach(var item in result)
                    {
                        Medicine medicine=await _medicineRepo.Get(item.MedicineId);
           
                        MedicationItem medicationItem = new MedicationItem()
                        {
                            MedicineId = item.MedicineId,
                            MedicationId = medication.MedicationId,
                            Quantity = item.Quantity

                        };
                        await _medicationItemRepo.Add(medicationItem);
                    

                    }
                    await _transactionService.CommitTransactionAsync();
                    SuccessMedicationDTO resultMedication = new SuccessMedicationDTO()
                    {
                        Code = 200,
                        Message = "Medication added successfully",
                        MedicationId = medication.MedicationId,
                    };
                    return resultMedication;

                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;

                }
            }

        }
        public async Task<SuccessMedicationDTO> UpdateMedication(UpdateMedication addMedication)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Customer customer = await _customerJoinedRepo.Get(addMedication.CustomerId);
                    Medicine medicine = await _medicineRepo.Get(addMedication.MedicineId);

                    Medication medication=await _medicationRepo.Get(addMedication.MedicationId);
                    MedicationItem? ExistingMedicationItem=medication.MedicationItems.FirstOrDefault(m=>m.MedicineId==addMedication.MedicineId);
                    if (ExistingMedicationItem == null)
                    {
                        MedicationItem item = new MedicationItem()
                        {
                            MedicationId = addMedication.MedicationId,
                            MedicineId = addMedication.MedicineId,
                            Quantity = addMedication.Quantity,
                        };
                        ExistingMedicationItem= await _medicationItemRepo.Add(item);
                    }
                    else
                    {
                        int finalQuantity = 0;
                        if (addMedication.Status == "Increase")
                        {
                            finalQuantity = addMedication.Quantity + ExistingMedicationItem.Quantity;
                        }
                        else
                        {
                            if (ExistingMedicationItem.Quantity > addMedication.Quantity)
                            {
                                finalQuantity = ExistingMedicationItem.Quantity - addMedication.Quantity;
                            }
                            else
                            {
                                throw new NegativeValueException("The expected quantity is not available in the medication");

                            }

                        }

                        if (medicine.CurrentQuantity < finalQuantity)
                        {
                            throw new OutOfStockException("Expected Quantity is not avalable in the stock");
                        }
                        ExistingMedicationItem.Quantity = finalQuantity;
                        await _medicationItemRepo.Update(ExistingMedicationItem);


                    }

                    await _transactionService.CommitTransactionAsync();
                    SuccessMedicationDTO success = new SuccessMedicationDTO()
                    {
                        Code = 200,
                        Message = "Item Updated Successfully",
                        MedicationId = ExistingMedicationItem.MedicationId

                    };

                    return success;

                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;

                }
            }

        }
       
        public async Task<SuccessCheckoutDTO> BuyMedication(int userId, int medicationId)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    List<MedicationItem> cart = (await _medicationRepo.Get(medicationId)).MedicationItems.ToList();

                    Order order = new Order()
                    {
                        CustomerId = userId,
                        Discount = 0,
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
                            throw new OutOfStockException("Expected Quantity is not avalable at the moment for " + stock.MedicineId);

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
                        await _medicineRepo.Update(stock);

                    }

                    order.TotalAmount = totalSum;
                    order.PaidAmount = totalSum;
                    await _orderRepo.Update(order);
                    await _transactionService.CommitTransactionAsync();
                    SuccessCheckoutDTO checkoutDTO = new SuccessCheckoutDTO()
                    {
                        Code = 200,
                        Message = "Medication Order Completed",
                        OrderId = order.OrderId,
                    };
                    return checkoutDTO;


                }
                catch
                {
                    await _transactionService.RollbackTransactionAsync();

                    throw;

                }

            }
        }



        }

    }
