namespace PharmacyManagementApi.CustomException
{
    public class OutOfStockException:Exception
    {
        string message;
        public OutOfStockException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
