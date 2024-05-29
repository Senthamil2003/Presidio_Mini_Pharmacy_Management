namespace PharmacyManagementApi.CustomException
{
    public class NoMedicationItemFoundException:Exception
    {
        string message;
        public NoMedicationItemFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
