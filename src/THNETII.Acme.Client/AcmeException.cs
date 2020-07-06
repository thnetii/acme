using System;
using System.Net;

namespace THNETII.Acme.Client
{
    /// <summary>
    /// Represents an error that occurred when an ACME server processed a request issued by an ACME client.
    /// </summary>
    public class AcmeException : Exception
    {
        private const string defaultMessageString = "An unexpected error occurred during an ACME request.";

        /// <summary>
        /// Gets the ACME error instance that is returned to the client.
        /// </summary>
        public AcmeError Error { get; }

        /// <summary>
        /// Gets the detailed message or the ACME error, or <c>null</c> if no details were specified in the error.
        /// </summary>
        public string Detail => Error?.Detail;

        /// <summary>
        /// The HTTP status code that is transmitted together with the ACME error.
        /// </summary>
        public HttpStatusCode StatusCode => Error?.StatusCode ?? default(HttpStatusCode);

        /// <summary>
        /// Creates a new generic ACME exception that has no ACME error instance associated with it.
        /// </summary>
        public AcmeException() : base(defaultMessageString) { }

        /// <summary>
        /// Creates a new ACME exception from the specified ACME error instance.
        /// </summary>
        /// <param name="error">The ACME error that is transmitted as the payload body to an ACME client instead of a successful response message.</param>
        public AcmeException(AcmeError error) : this(defaultMessageString, error) { }

        /// <summary>
        /// Creates a new ACME exception with a custom message from the specified ACME error instance.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="error">The ACME error that is transmitted as the payload body to an ACME client instead of a successful response message.</param>
        public AcmeException(string message, AcmeError error) : base(CreateFullMessageString(message, error))
        {
            Error = error;
        }

        /// <summary>
        /// Creates a new generic ACME exception with a reference to the specified inner exceoption that is the cause of this exception.
        /// </summary>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.</param>
        public AcmeException(Exception innerException) : base(defaultMessageString, innerException) { }

        /// <summary>
        /// Creates a new ACME exception from the specified ACME error instance and a reference to an inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="error">The ACME error that is transmitted as the payload body to an ACME client instead of a successful response message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.</param>
        /// <remarks><paramref name="innerException"/> will typically be a reference to the exception that is thrown when the client validates the status code of the received response message.</remarks>
        public AcmeException(AcmeError error, Exception innerException) : this(defaultMessageString, error, innerException) { }

        /// <summary>
        /// Creates a new ACME exception with a custom message from the specified ACME error instance and a reference to the specified inner exceoption that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="error">The ACME error that is transmitted as the payload body to an ACME client instead of a successful response message.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.</param>
        public AcmeException(string message, AcmeError error, Exception innerException) : base(CreateFullMessageString(message, error), innerException)
        {
            Error = error;
        }

        private static string CreateFullMessageString(string message, AcmeError error)
        {
            string firstLine = string.IsNullOrWhiteSpace(message) ? defaultMessageString : message;
            if (string.IsNullOrWhiteSpace(error.Detail))
                return firstLine;
            return $"{firstLine}{Environment.NewLine}Detail: {error.Detail}";
        }
    }
}
