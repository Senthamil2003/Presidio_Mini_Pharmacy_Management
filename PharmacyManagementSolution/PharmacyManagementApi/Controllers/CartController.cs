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

        public CartController(ICartService userService) {
            _userService=userService;
        }

        [HttpPost("AddToCart")]
        [ProducesResponseType(typeof(SuccessCartDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCartDTO>> AddToCart(AddToCartDTO addToCart)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                addToCart.UserId = userid;
                var result = await _userService.AddToCart(addToCart);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
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
        public async Task<ActionResult<SuccessCartDTO>> UpdateCartItem(UpdateCartDTO addToCart)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                addToCart.UserId = userid;  
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
        public async Task<ActionResult<SuccessCheckoutDTO>> Checkout()
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
               
                var result = await _userService.Checkout(userid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
