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
      
        public UserService(StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            IReposiroty<int ,Order> orderRepo,
            IReposiroty<int, OrderDetail> orderDetailRepo,
            
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
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
               try
                {
                    Medicine medicine = await _medicineRepo.Get(addToCart.MedicineId);
                    var result= await FindMedicineById(medicine.MedicineId);
                    if (result.AvailableQuantity < addToCart.Quantity)
                    {
                        throw new OutOfStockException("Expected Quantity is not avalable in the stock");
                    }
                    Cart? ExistingCart= (await _cartRepo.Get()).FirstOrDefault(c=>c.MedicineId== addToCart.MedicineId);
                    if(ExistingCart!= null)
                    {
                        throw new DuplicateValueException("There is already a cart with medicine Id try updateCart" + addToCart.MedicineId);
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
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;

                }
            }
           
        }
        public async Task<SuccessCartDTO> UpdateCart(AddToCartDTO addToCart)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Medicine medicine = await _medicineRepo.Get(addToCart.MedicineId);
                    var result = await FindMedicineById(medicine.MedicineId);
                   Customer customer= await _customer.Get(addToCart.UserId);
                   Cart? ExistingCart= customer.Carts.FirstOrDefault(c=>c.MedicineId==addToCart.MedicineId);
                    if(ExistingCart== null) {
                        throw new NoCartFoundException("No cart Contains the Medicine Id , Add the cart first");
                    }
                    
                    if (result.AvailableQuantity < addToCart.Quantity+ExistingCart?.Quantity)
                    {
                        throw new OutOfStockException("Expected Quantity is not avalable in the stock");
                    }
                    ExistingCart.Quantity += addToCart.Quantity;
                    await _cartRepo.Update(ExistingCart);

                    await _transactionService.CommitTransactionAsync();
                    SuccessCartDTO success = new SuccessCartDTO()
                    {
                        Code = 200,
                        Message = "Item Added Successfully",
                        CartId = ExistingCart.CartId

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

        public async Task<SuccessCartDTO> RemoveFromCart(int cartId)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
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
                catch
                {
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }

            }


        }

        public async Task<SuccessCheckoutDTO> Checkout(int userId)
        {
            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
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
                    double totalSum = 0;
                    foreach (Cart item in cart)
                    {
                        totalSum += item.Cost;
                        var stock = await _medicineRepo.Get(item.MedicineId);
                        if (stock.CurrentQuantity < item.Quantity)
                        {
                            throw new OutOfStockException("Expected Quantity is not avalable at the moment for "+stock.MedicineId);

                        }
                        OrderDetail orderDetail = new OrderDetail()
                        {
                            MedicineId = item.MedicineId,
                            Cost = stock.SellingPrice,
                            OrderId = order.OrderId,
                            Quantity=item.Quantity

                        };
                        await _orderDetailRepo.Add(orderDetail);
                        stock.CurrentQuantity -= item.Quantity;
                        await _medicineRepo.Update(stock);
                       await _cartRepo.Delete(item.CartId);


                    }
                    order.TotalAmount=totalSum;
                    order.PaidAmount = totalSum;
                   await _orderRepo.Update(order);
                    await _transactionService.CommitTransactionAsync();
                    SuccessCheckoutDTO checkoutDTO = new SuccessCheckoutDTO()
                    {
                        Code = 200,
                        Message = "Checkout Completed successfully",
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
        public async Task<List<MyOrderDTO>> GetAllOrders(int userId)
        {
            try
            {
                Customer customer = await _customer.Get(userId);
                List<MyOrderDTO> myOrders = new List<MyOrderDTO>(); 
                List<Order> orders=customer.Orders.ToList();
                if(orders.Count == 0) {
                    throw new NoOrderFoundException("There is no order for the customer");
                }
                foreach (var item in orders)
                {
                    List<OrderDetail> orderDetails = item.OrderDetails.ToList();
                    foreach (var orderDetail in orderDetails) {
                        List<DeliveryDetail> deliveryDetails = orderDetail.DeliveryDetails.ToList();
                        foreach(var deliveryDetail in deliveryDetails) {
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
