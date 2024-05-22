namespace PharmacyManagementApi.CustomException
{
    public class NoUserCredentialFoundException:Exception
    {
        string message;
        public NoUserCredentialFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
