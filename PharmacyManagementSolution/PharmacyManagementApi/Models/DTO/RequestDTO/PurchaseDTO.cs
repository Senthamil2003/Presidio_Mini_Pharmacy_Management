namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class PurchaseDTO
    {
       public  DateTime DateTime { get; set; }
       public PurchaseItem[] Items { get; set; }
   
    }
}
