namespace PharmacyManagementApi.CustomException
{
    public class NoMedicationFoundException:Exception
    {
        string message;
        public NoMedicationFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
