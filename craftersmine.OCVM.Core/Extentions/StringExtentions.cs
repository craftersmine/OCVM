using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Extentions
{
    public static class StringExtentions
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
