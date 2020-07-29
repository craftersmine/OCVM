using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Exceptions
{

    [Serializable]
    public class BitBltBufferOperationException : Exception
    {
        public BitBltBufferOperationException() { }
        public BitBltBufferOperationException(string message) : base(message) { }
        public BitBltBufferOperationException(string message, Exception inner) : base(message, inner) { }
        protected BitBltBufferOperationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
