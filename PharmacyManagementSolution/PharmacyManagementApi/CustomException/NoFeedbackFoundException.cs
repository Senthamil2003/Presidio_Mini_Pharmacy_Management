namespace PharmacyManagementApi.CustomException
{
    public class NoFeedbackFoundException:Exception
    {
        string message;
        public NoFeedbackFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
