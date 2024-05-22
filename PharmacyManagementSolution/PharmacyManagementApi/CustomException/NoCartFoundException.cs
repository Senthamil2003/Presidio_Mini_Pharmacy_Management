namespace PharmacyManagementApi.CustomException
{
    public class NoCartFoundException:Exception
    {
        string message;
        public NoCartFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
