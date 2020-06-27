using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Exceptions
{

    [Serializable]
    public class EEPROMFailedToLoadException : Exception
    {
        public EEPROMFailedToLoadException() { }
        public EEPROMFailedToLoadException(string message) : base(message) { }
        public EEPROMFailedToLoadException(string message, Exception inner) : base(message, inner) { }
        protected EEPROMFailedToLoadException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
