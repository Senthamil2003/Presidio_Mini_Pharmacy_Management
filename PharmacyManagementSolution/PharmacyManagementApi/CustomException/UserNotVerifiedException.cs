namespace PharmacyManagementApi.CustomException
{
    public class UserNotVerifiedException:Exception
    {
        string message;
        public UserNotVerifiedException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
