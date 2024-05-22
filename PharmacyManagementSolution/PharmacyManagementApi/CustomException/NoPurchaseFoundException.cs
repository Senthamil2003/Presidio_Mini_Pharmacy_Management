namespace PharmacyManagementApi.CustomException
{
    public class NoPurchaseFoundException:Exception
    {
        string message;
        public NoPurchaseFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
