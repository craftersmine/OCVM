using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullEmptyOrWhitespace(this string str)
        {
            if (str != null)
            {
                if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                    return false;
            }
            return true;
        }
    }
}
