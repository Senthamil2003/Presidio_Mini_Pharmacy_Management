using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Repositories.Joined_Repositories;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging; // Added for logging

namespace PharmacyManagementApi.Services
{
    public class CartService : ICartService
    {
        private readonly StockJoinedRepository _stockRepo;
        private readonly ITransactionService _transactionService;
        private readonly IRepository<int, Medicine> _medicineRepo;
        private readonly IRepository<int, Cart> _cartRepo;
        private readonly CustomerJoinedRepository _customer;
        private readonly IRepository<int, Order> _orderRepo;
        private readonly IRepository<int, OrderDetail> _orderDetailRepo;
        private readonly ILogger<CartService> _logger; // Added for logging

        public CartService(StockJoinedRepository stockJoinedRepo,
            CustomerJoinedRepository customerJoinRepo,
            IRepository<int, Order> orderRepo,
            IRepository<int, OrderDetail> orderDetailRepo,
            ITransactionService transactionService,
            IRepository<int, Medicine> medicineRepo,
            IRepository<int, Cart> cartRepo,
            ILogger<CartService> logger) // Added logger parameter
        {
            _stockRepo = stockJoinedRepo;
            _transactionService = transactionService;
            _medicineRepo = medicineRepo;
            _cartRepo = cartRepo;
            _customer = customerJoinRepo;
            _orderRepo = orderRepo;
            _orderDetailRepo = orderDetailRepo;
            _logger = logger; 
        }

        [ExcludeFromCodeCoverage]
        public async Task<StockResponseDTO> FindMedicineById(int MedicineId)
        {
            _logger.LogInformation("Finding medicine details for medicine ID: {MedicineId}", MedicineId); // Log start

            try
            {
                Medicine result = await _medicineRepo.Get(MedicineId);

                if (result != null)
                {
                    StockResponseDTO response = new StockResponseDTO()
                    {
                        MedicineId = result.MedicineId,
                        MedicineName = result.MedicineName,
                        Category = result.Category.CategoryName,
                        Amount = result.SellingPrice,
                        AvailableQuantity = result.CurrentQuantity
                    };

                    _logger.LogInformation("Medicine details found for medicine ID: {MedicineId}", MedicineId); // Log success
                    return response;
                }

                _logger.LogWarning("Medicine is out of stock for ID: {MedicineId}", MedicineId); // Log warning
                throw new OutOfStockException("Out of stock for the medicine Id " + MedicineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding medicine details for ID: {MedicineId}", MedicineId); // Log error
                throw;
            }
        }

        /// <summary>
        /// Adds a medicine to the cart for a customer.
        /// </summary>
        /// <param name="addToCart">The details of the medicine and quantity to add to the cart.</param>
        /// <returns>A success DTO containing the cart ID.</returns>
        public async Task<SuccessCartDTO> AddToCart(AddToCartDTO addToCart)
        {
            _logger.LogInformation("Adding medicine {MedicineId} to cart for customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log start

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                 
                    var result = await FindMedicineById(addToCart.MedicineId);
                    if (result.AvailableQuantity < addToCart.Quantity)
                    {
                        _logger.LogWarning("Expected quantity {ExpectedQuantity} is not available for medicine {MedicineId}", addToCart.Quantity, addToCart.MedicineId); // Log warning
                        throw new OutOfStockException("Expected Quantity is not available in the stock");
                    }

                    Cart? ExistingCart = (await _cartRepo.Get()).FirstOrDefault(c => (c.MedicineId == addToCart.MedicineId && c.CustomerId == addToCart.UserId));
                    if (ExistingCart != null)
                    {
                        _logger.LogWarning("Cart already exists for medicine {MedicineId} and customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log warning
                        throw new DuplicateValueException("There is already a cart with medicine Id try updateCart" + addToCart.MedicineId);
                    }

                    Cart cart = new Cart()
                    {
                        CustomerId = addToCart.UserId,
                        Quantity = addToCart.Quantity,
                        MedicineId = result.MedicineId,
                        Cost = result.Amount,
                        TotalCost = result.Amount * addToCart.Quantity
                    };

                    await _cartRepo.Add(cart);
                    await _transactionService.CommitTransactionAsync();

                    SuccessCartDTO success = new SuccessCartDTO()
                    {
                        Code = 200,
                        Message = "Item Added Successfully",
                        CartId = cart.CartId
                    };

                    _logger.LogInformation("Medicine {MedicineId} added to cart {CartId} for customer {UserId}", addToCart.MedicineId, cart.CartId, addToCart.UserId); // Log success
                    return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error adding medicine {MedicineId} to cart for customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log error
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates the quantity of a medicine in the cart for a customer.
        /// </summary>
        /// <param name="addToCart">The details of the medicine ID, customer ID, and quantity to update.</param>
        /// <returns>A success DTO containing the cart ID.</returns>
        public async Task<SuccessCartDTO> UpdateCart(UpdateCartDTO addToCart)
        {
            _logger.LogInformation("Updating cart for medicine {MedicineId} and customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log start

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Medicine medicine = await _medicineRepo.Get(addToCart.MedicineId);
                    var result = await FindMedicineById(medicine.MedicineId);
                    Customer customer = await _customer.Get(addToCart.UserId);
                    Cart? ExistingCart = customer.Carts.FirstOrDefault(c => c.MedicineId == addToCart.MedicineId);
                    if (ExistingCart == null)
                    {
                        _logger.LogWarning("No cart found for medicine {MedicineId} and customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log warning
                        throw new NoCartFoundException("No cart Contains the Medicine Id , Add the cart first");
                    }

                    int finalQuantity = 0;
                    if (addToCart.Status == "Increase")
                    {
                        finalQuantity = addToCart.Quantity + ExistingCart.Quantity;
                    }
                    else
                    {
                        if (ExistingCart.Quantity > addToCart.Quantity)
                        {
                            finalQuantity = ExistingCart.Quantity - addToCart.Quantity;
                        }
                        else
                        {
                            _logger.LogWarning("The expected quantity {ExpectedQuantity} is not available in the cart for medicine {MedicineId}", addToCart.Quantity, addToCart.MedicineId); // Log warning
                            throw new NegativeValueException("The expected quantity is not available in the cart");
                        }
                    }

                    if (result.AvailableQuantity < finalQuantity)
                    {
                        _logger.LogWarning("Expected quantity {ExpectedQuantity} is not available in stock for medicine {MedicineId}", finalQuantity, addToCart.MedicineId); // Log warning
                        throw new OutOfStockException("Expected Quantity is not available in the stock");
                    }

                    ExistingCart.Quantity = finalQuantity;
                    await _cartRepo.Update(ExistingCart);

                    await _transactionService.CommitTransactionAsync();

                    SuccessCartDTO success = new SuccessCartDTO()
                    {
                        Code = 200,
                        Message = "Item Updated Successfully",
                        CartId = ExistingCart.CartId
                    };

                    _logger.LogInformation("Cart updated successfully for medicine {MedicineId} and customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log success
                    return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating cart for medicine {MedicineId} and customer {UserId}", addToCart.MedicineId, addToCart.UserId); // Log error
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes a medicine from the cart for a customer.
        /// </summary>
        /// <param name="cartId">The cart ID of the item to remove.</param>
        /// <returns>A success DTO containing the cart ID.</returns>
        public async Task<SuccessCartDTO> RemoveFromCart(int cartId,int userId)
        {
            _logger.LogInformation("Removing cart item with ID: {CartId}", cartId); // Log start

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    Cart cart = await _cartRepo.Get(cartId);
                    if (cart.CustomerId != userId)
                    {
                        throw new NoCartFoundException("The user have no such cart");
                    }
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

                    _logger.LogInformation("Cart item with ID: {CartId} removed successfully", cartId); // Log success
                    return success;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error removing cart item with ID: {CartId}", cartId); // Log error
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Performs the checkout process for a customer's cart.
        /// </summary>
        /// <param name="userId">The customer ID.</param>
        /// <returns>A success DTO containing the order ID.</returns>
        public async Task<SuccessCheckoutDTO> Checkout(int userId)
        {
            _logger.LogInformation("Performing checkout for customer {UserId}", userId); // Log start

            using (var transaction = await _transactionService.BeginTransactionAsync())
            {
                try
                {
                    List<Cart> cart = (await _customer.Get(userId)).Carts.ToList();
                    if (cart.Count() == 0)
                    {
                        _logger.LogWarning("Cart is empty for customer {UserId}", userId); // Log warning
                        throw new CartEmptyException("Your Cart is Empty");
                    }

                    bool customerSubscribe = (await _customer.Get(userId)).IsSubcribed;
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
                    foreach (Cart item in cart)
                    {
                        totalSum += item.Cost*item.Quantity;
                        var stock = await _medicineRepo.Get(item.MedicineId);
                        if (stock.CurrentQuantity < item.Quantity)
                        {
                            _logger.LogWarning("Expected quantity {ExpectedQuantity} is not available in stock for medicine {MedicineId}", item.Quantity, stock.MedicineId); // Log warning
                            throw new OutOfStockException("Expected Quantity is not available at the moment for " + stock.MedicineId);
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
                        await _cartRepo.Delete(item.CartId);
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
                        Message = "Checkout Completed successfully",
                        OrderId = order.OrderId,
                    };

                    _logger.LogInformation("Checkout completed successfully for customer {UserId}, order ID: {OrderId}", userId, order.OrderId); // Log success
                    return checkoutDTO;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during checkout for customer {UserId}", userId); // Log error
                    await _transactionService.RollbackTransactionAsync();
                    throw;
                }
            }
        }
    }
}