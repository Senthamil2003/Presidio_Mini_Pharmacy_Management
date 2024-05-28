using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
    public class NoPurchaseDetailFoundException:Exception
    {
        string message;
        public NoPurchaseDetailFoundException(string message)
        {
            this.message = message;
        }
        public override string Message =>message;
    }
}
