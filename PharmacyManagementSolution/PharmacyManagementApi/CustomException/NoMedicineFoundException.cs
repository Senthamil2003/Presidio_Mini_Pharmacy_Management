using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.CustomException
{
 
    public class NoMedicineFoundException:Exception
    {
        string message;
        public NoMedicineFoundException(string message)
        {
            this.message = message;
        }
        public override string Message => message;
    }
}
