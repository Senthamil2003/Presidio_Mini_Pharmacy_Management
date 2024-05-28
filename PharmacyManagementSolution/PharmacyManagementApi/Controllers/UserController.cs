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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) {
            _userService=userService;
        }
        [HttpGet("ViewAllItems")]
        [ProducesResponseType(typeof(StockResponseDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StockResponseDTO[]>> GetAllProduct()
        {
            try
            {
                var result = await _userService.ShowAllProduct();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPost("AddToCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> AddToCart(AddToCartDTO addToCart)
        {
            try
            {
                var result = await _userService.AddToCart(addToCart);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpDelete("RemoveFromCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> RemoveFromCart(int CartId)
        {
            try
            {
                var result = await _userService.RemoveFromCart(CartId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPut("UpdateCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> UpdateCartItem(AddToCartDTO addToCart)
        {
            try
            {
                var result = await _userService.UpdateCart(addToCart);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPost("Checkout")]
        [ProducesResponseType(typeof(SuccessCheckoutDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCheckoutDTO>> Checkout(int UserId)
        {
            try
            {
                var result = await _userService.Checkout(UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPost("AllOrder")]
        [ProducesResponseType(typeof(List<MyOrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MyOrderDTO>>> AllOrder(int UserId)
        {
            try
            {
                var result = await _userService.GetAllOrders(UserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
