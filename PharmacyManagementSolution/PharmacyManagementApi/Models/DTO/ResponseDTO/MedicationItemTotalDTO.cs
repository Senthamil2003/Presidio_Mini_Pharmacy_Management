namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class MedicationItemTotalDTO
    {
        public string MedicationName { get; set; }
       public  List<MedicationItemDetailDTO> medicationItemDetailDTOs { get; set; }
    }
}
