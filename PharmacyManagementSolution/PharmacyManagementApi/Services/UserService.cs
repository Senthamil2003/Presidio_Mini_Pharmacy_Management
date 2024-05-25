using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;

namespace PharmacyManagementApi.Services
{
    public class UserService:IUserService
    {
        private readonly IReposiroty<int, Stock> _stockRepo;
        private readonly ITransactionService _transactionService;
        private readonly IReposiroty<int, Medicine> _medicineRepo;
        private readonly IReposiroty<int, Cart> _cartRepo;
        private readonly CustomerJoinedRepository _customer;
        private readonly IReposiroty<int, Order> _orderRepo;
        private readonly IReposiroty<int, OrderDetail> _orderDetailRepo;
        private readonly IReposiroty<int, DeliveryDetail> _deliveryDetailRepo;

        public UserService(StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            IReposiroty<int ,Order> orderRepo,
            IReposiroty<int, OrderDetail> orderDetailRepo,
            IReposiroty<int,DeliveryDetail> deliveryDetailRepo,
            ITransactionService transactionService,
            IReposiroty<int,Medicine> medicineRepo,
            IReposiroty<int, Cart> cartRepo

            ) {

            _stockRepo=stockJoinedRepo;     
            _transactionService=transactionService;
            _medicineRepo = medicineRepo;
            _cartRepo = cartRepo;
            _customer=customerJoinRepo;
            _orderRepo=orderRepo;
            _orderDetailRepo=orderDetailRepo;
            _deliveryDetailRepo=deliveryDetailRepo;
        }
        public async Task<StockResponseDTO[]> ShowAllProduct()
        {
            try
            {
              var result=  (await _stockRepo.Get()).GroupBy(s => s.MedicineId).Select(item => new
                {
                   MedicineId = item.Key,
                   TotalQuantity = item.Sum(i=>i.Quantity),
                   SellingPrice =item.Max(i=>i.SellingPrice),
                   MedicineName =item.First().Medicine.MedicineName,
                   CategoryName =item.First().Medicine.Category.CategoryName
                });
                StockResponseDTO[] responseDTO = new StockResponseDTO[result.Count()];
                int ct = 0;
                foreach (var item in result) {
                    StockResponseDTO response = new StockResponseDTO()
                    {
                        MedicineId = item.MedicineId,
                        MedicineName=item.MedicineName,
                        Category=item.CategoryName,
                        Amount=item.SellingPrice,
                        AvailableQuantity=item.TotalQuantity

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
        public async Task<StockResponseDTO> FindMedicineById(int MedicineId)
        {
            try
            {
                var result = (await _stockRepo.Get()).Where(s => s.MedicineId == MedicineId).GroupBy(s => s.MedicineId).Select(group => new
                {
                    MedicineId = group.Key,
                    TotalQuantity = group.Sum(i => i.Quantity),
                    SellingPrice = group.Max(i => i.SellingPrice),
                    MedicineName = group.First().Medicine.MedicineName,
                    CategoryName = group.First().Medicine.Category.CategoryName
                }).FirstOrDefault();

                if (result != null)
                {
                    StockResponseDTO response = new StockResponseDTO()
                    {
                        MedicineId = result.MedicineId,
                        MedicineName = result.MedicineName,
                        Category = result.CategoryName,
                        Amount = result.SellingPrice,
                        AvailableQuantity = result.TotalQuantity
                    };

                    return response;
                }
                throw new OutOfStockException("Out of stock for the medicine Id "+MedicineId);


            }
            catch
            {
                throw; 
            }
        }


        public async Task<SuccessCartDTO> AddToCart(AddToCartDTO addToCart)
        {
            try
            {
                using (var transaction = await _transactionService.BeginTransactionAsync())
                {
                    Medicine medicine = await _medicineRepo.Get(addToCart.MedicineId);
                    var result= await FindMedicineById(medicine.MedicineId);
                    if (result.AvailableQuantity < addToCart.Quantity)
                    {
                        throw new OutOfStockException("Expected Quantity is not avalable in the stock");
                    }
                    Cart cart =new Cart()
                    {
                        CustomerId = addToCart.UserId,
                        Quantity = addToCart.Quantity,
                        MedicineId = medicine.MedicineId,
                        Cost=result.Amount,
                        TotalCost= result.Amount* addToCart.Quantity

                    };
                   await _cartRepo.Add(cart);
                    
                   await _transactionService.CommitTransactionAsync();
                    SuccessCartDTO success = new SuccessCartDTO()
                    {
                        Code = 200,
                        Message = "Item Added Successfully",
                        CartId = cart.CartId

                    };
                    return success;

                }
            }
            catch
            {
                await _transactionService.RollbackTransactionAsync(); 
                throw;   

            }
        }

        public async Task<SuccessCartDTO> RemoveFromCart(int cartId)
        {
            try
            {
                using (var transaction = await _transactionService.BeginTransactionAsync())
                {
                    Cart cart = await _cartRepo.Get(cartId);
                    if (cart != null)
                    {
                        await _cartRepo.Delete(cartId);
                    }
                   await _transactionService.CommitTransactionAsync();
                    SuccessCartDTO success = new SuccessCartDTO()
                    {
                        Code = 200,
                        Message = "Cart Deleted Successfully",
                        CartId = cartId
                    };
                    return success;

                }

            }
            catch
            {
               await _transactionService.RollbackTransactionAsync();
                throw;
            }

        }
        public async Task Checkout(int userId)
        {
            try
            {
                using (var transaction = await _transactionService.BeginTransactionAsync())
                {
                    List<Cart> cart = (await _customer.Get(userId)).Carts.ToList();
                    if (cart.Count() == 0)
                    {
                        throw new CartEmptyException("Your Cart is Empty");
                    }
                    Order order = new Order()
                    {
                        CustomerId = userId,
                        Discount = 0,
                        ShipmentCost = 0,
                        TotalAmount = 0,
                        PaidAmount = 0,
                    };
                    await _orderRepo.Add(order);
                    foreach (Cart item in cart)
                    {
                        var stock = await FindMedicineById(item.MedicineId);
                        if (stock.AvailableQuantity < item.Quantity)
                        {
                            throw new OutOfStockException("Expected Quantity is not avalable in the stock");

                        }
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            MedicineId = item.MedicineId,
                            Cost = stock.Amount,
                            OrderId = order.OrderId,

                        };
                        await _orderDetailRepo.Add(orderDetail);

                        var result = (await _stockRepo.Get()).OrderByDescending(s => s.ExpiryDate).ToList();
                        int orderQuantity = item.Quantity;
                        int ct = 0;
                        while (orderQuantity > 0)
                        {
                            Stock updateStock = await _stockRepo.Get(result[ct].StockId);
                            if (result[ct].Quantity > orderQuantity)
                            {
                                updateStock.Quantity -= orderQuantity;
                                orderQuantity = 0;
                                await _stockRepo.Update(updateStock);
                            }
                            else
                            {
                                orderQuantity -= result[ct].Quantity;
                                await _stockRepo.Delete(updateStock.StockId);

                            }
                            DeliveryDetail delivery = new DeliveryDetail()
                            {
                                CustomerId = userId,
                                ExpiryDate = updateStock.ExpiryDate,
                                MedicineId = item.MedicineId,
                                OrderDetailId = orderDetail.OrderId,
                            };
                            await _deliveryDetailRepo.Add(delivery);
                            ct++;
                        }
                        await _transactionService.CommitTransactionAsync();



                    }


                }

            }
            catch
            {
                await _transactionService.RollbackTransactionAsync();
                throw;

            }

        }

    }
}
