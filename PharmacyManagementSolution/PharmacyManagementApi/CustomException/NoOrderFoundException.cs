namespace PharmacyManagementApi.CustomException
{
    public class NoOrderFoundException:Exception
    {
        string message;
        public NoOrderFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
