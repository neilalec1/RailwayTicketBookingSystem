using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;

namespace CW2
{
    public class NonNegativeIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (int.TryParse(value as string, out int intValue))
            {
                if (intValue >= 0)
                {
                    return ValidationResult.ValidResult;
                }
                else
                {
                    return new ValidationResult(false, "Value must be a non-negative integer.");
                }
            }
            else
            {
                return new ValidationResult(false, "Invalid input. Value must be an integer.");
            }
        }
    }
}
