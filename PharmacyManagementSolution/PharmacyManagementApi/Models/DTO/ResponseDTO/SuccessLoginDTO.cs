namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class SuccessLoginDTO
    {
        public int Code { get; set; }
        public string Role { get; set; } 
        public string AccessToken { get; set; }

    }
}
