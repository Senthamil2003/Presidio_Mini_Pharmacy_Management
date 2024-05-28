using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
  
    public class CartEmptyException:Exception
    {
        string message;
        public CartEmptyException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
