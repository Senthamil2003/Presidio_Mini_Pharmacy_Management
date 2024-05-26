using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IUserService
    {
        public Task<StockResponseDTO[]> ShowAllProduct();
        public Task<SuccessCartDTO> AddToCart(AddToCartDTO addToCart);
        public  Task<SuccessCartDTO> RemoveFromCart(int cartId);
        public  Task<string> Checkout(int userId);
    }
}
