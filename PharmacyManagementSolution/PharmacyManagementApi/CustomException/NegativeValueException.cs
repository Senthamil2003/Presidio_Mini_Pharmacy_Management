namespace PharmacyManagementApi.CustomException
{
    public class NegativeValueException:Exception
    {
        string message;
        public NegativeValueException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
