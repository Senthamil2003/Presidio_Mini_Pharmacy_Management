namespace PharmacyManagementApi.CustomException
{
    public class NoOrderDetailFoundException:Exception
    {
        string message;
        public NoOrderDetailFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
