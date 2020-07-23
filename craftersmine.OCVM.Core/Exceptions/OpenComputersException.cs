using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Exceptions
{
    [Serializable]
    public class OpenComputersException : Exception
    {
        public OpenComputersException() { }
        public OpenComputersException(string message) : base(message) { }
        public OpenComputersException(string message, Exception inner) : base(message, inner) { }
        protected OpenComputersException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
