using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Attributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class OpenComputersComponentAttribute : Attribute
    {
        public OpenComputersComponentAttribute()
        {
        }

        public string ComponentType { get; set; } = "generic";
    }
}
