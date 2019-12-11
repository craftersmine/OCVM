using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace craftersmine.OCVM.Core.Exceptions
{

    /// <summary>
    /// The exception that is thrown when an attempt to connect device to bus with exceded bus lanes
    /// </summary>
    [Serializable]
    public class ExcededBusLanesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcededBusLanesException"/> class with its message string set to a system-supplied message.
        /// </summary>
        public ExcededBusLanesException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcededBusLanesException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public ExcededBusLanesException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcededBusLanesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">A description of the error. The content of message is intended to be understood by humans. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        /// <param name="inner">The exception that is the cause of the current exception. If the innerException parameter is not null, the current exception is raised in a catch block that handles the inner exception.</param>
        public ExcededBusLanesException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcededBusLanesException"/> class with the specified serialization and context information.
        /// </summary>
        /// <param name="info">An object that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">An object that contains contextual information about the source or destination.</param>
        protected ExcededBusLanesException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
