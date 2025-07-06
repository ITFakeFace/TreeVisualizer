using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultipleChoice.Utils
{
    class ValidationHelper
    {
        public static bool ValidateEmailFormat(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        public static bool ValidateUnsignedNumberFormat(string number)
        {
            string pattern = @"^[0-9]+$";
            return Regex.IsMatch(number, pattern);
        }
    }
}
