namespace PharmacyManagementApi.CustomException
{
    public class DuplicateValueException:Exception
    {
        string message;
        public DuplicateValueException(string message)
        {
            this.message = message;
        }
        public override string Message =>message;
    }
}
