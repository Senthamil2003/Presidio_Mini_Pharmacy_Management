namespace PharmacyManagementApi.CustomException
{
    public class NoVendorFoundException:Exception
    {
        string message;
        public NoVendorFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
