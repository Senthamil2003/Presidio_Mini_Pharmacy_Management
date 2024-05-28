using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
   
    public class NoDeliveryDetailFoundException:Exception
    {
        string message;
        public NoDeliveryDetailFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
