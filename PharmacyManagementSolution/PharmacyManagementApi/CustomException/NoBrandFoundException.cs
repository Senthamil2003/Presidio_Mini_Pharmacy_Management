namespace PharmacyManagementApi.CustomException
{
    public class NoBrandFoundException:Exception
    {
        string message;
        public NoBrandFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
