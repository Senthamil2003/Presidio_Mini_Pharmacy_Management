namespace PharmacyManagementApi.CustomException
{
    public class UnAuthorizedUserException:Exception
    {
        string message;
        public UnAuthorizedUserException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
