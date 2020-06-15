using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Exceptions
{

    [Serializable]
    public class InvalidDeviceException : Exception
    {
        public InvalidDeviceException() { }
        public InvalidDeviceException(string message) : base(message) { }
        public InvalidDeviceException(string message, Exception inner) : base(message, inner) { }
        protected InvalidDeviceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
