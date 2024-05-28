using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
  
    public class CategoryMedicineMisMatchException:Exception
    {
        string message;
        public CategoryMedicineMisMatchException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
