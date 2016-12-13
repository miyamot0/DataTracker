using System.Globalization;
using System.Windows.Controls;

namespace DataTracker.View
{
    class SessionValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int session;
            
            if(int.TryParse(value.ToString(), out session))
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Value is not a number");
            }
        }
    }
}
