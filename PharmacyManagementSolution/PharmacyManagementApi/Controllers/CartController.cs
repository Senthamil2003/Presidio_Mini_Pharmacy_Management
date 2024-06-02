using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _userService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService userService, ILogger<CartController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Adds an item to the user's cart.
        /// </summary>
        /// <param name="addToCart">The details of the item to be added to the cart.</param>
        /// <returns>A success response with the updated cart details.</returns>
        [HttpPost("AddToCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> AddToCart(AddToCartDTO addToCart)
        {
            try
            {
                _logger.LogInformation("Received a request to add an item to the cart.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the add to cart request.");
                    return BadRequest(ModelState);
                }

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                addToCart.UserId = userid;
                var result = await _userService.AddToCart(addToCart);
                _logger.LogInformation("Item added to the cart successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding an item to the cart: {ex.Message}");
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Removes an item from the user's cart.
        /// </summary>
        /// <param name="CartId">The ID of the cart item to be removed.</param>
        /// <returns>A success response with the updated cart details.</returns>
        [HttpDelete("RemoveFromCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> RemoveFromCart(int CartId)
        {
            try
            {
                _logger.LogInformation($"Received a request to remove an item with ID {CartId} from the cart.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the remove from cart request.");
                    return BadRequest(ModelState);
                }

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _userService.RemoveFromCart(CartId,userid);
                _logger.LogInformation("Item removed from the cart successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while removing an item from the cart: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Updates the quantity of an item in the user's cart.
        /// </summary>
        /// <param name="addToCart">The updated details of the cart item.</param>
        /// <returns>A success response with the updated cart details.</returns>
        [HttpPut("UpdateCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> UpdateCartItem(UpdateCartDTO addToCart)
        {
            try
            {
                _logger.LogInformation("Received a request to update a cart item.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the update cart request.");
                    return BadRequest(ModelState);
                }

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                addToCart.UserId = userid;
                var result = await _userService.UpdateCart(addToCart);
                _logger.LogInformation("Cart item updated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating a cart item: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Checks out the user's cart and places an order.
        /// </summary>
        /// <returns>A success response with the order details.</returns>
        [HttpPost("Checkout")]
        [ProducesResponseType(typeof(SuccessCheckoutDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCheckoutDTO>> Checkout()
        {
            try
            {
                _logger.LogInformation("Received a request to checkout the cart.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the checkout request.");
                    return BadRequest(ModelState);
                }

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);

                var result = await _userService.Checkout(userid);
                _logger.LogInformation("Checkout successful.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during checkout: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}