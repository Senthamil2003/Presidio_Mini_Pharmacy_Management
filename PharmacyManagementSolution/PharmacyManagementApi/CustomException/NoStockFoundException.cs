namespace PharmacyManagementApi.CustomException
{
    public class NoStockFoundException:Exception
    {
        string message;
        public NoStockFoundException(string message)
        {
            this.message = message;
        }
        public override string Message =>message;

    }
}
