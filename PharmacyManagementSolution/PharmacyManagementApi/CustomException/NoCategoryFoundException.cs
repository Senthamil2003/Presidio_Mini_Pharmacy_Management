using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
    public class NoCategoryFoundException:Exception
    {
        string message;
        public NoCategoryFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
