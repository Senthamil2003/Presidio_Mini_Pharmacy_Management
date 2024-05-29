using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface ICartService
    {
     
        public Task<SuccessCartDTO> AddToCart(AddToCartDTO addToCart);
        public  Task<SuccessCartDTO> RemoveFromCart(int cartId);
        public  Task<SuccessCheckoutDTO> Checkout(int userId);
      
        public Task<SuccessCartDTO> UpdateCart(UpdateCartDTO addToCart);
    }
}
