using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class LuaCallbackAttribute : Attribute
    {
        public LuaCallbackAttribute()
        {
        }

        public bool IsDirect { get; set; } = false;
        public string Doc { get; set; } = "";
    }
}
