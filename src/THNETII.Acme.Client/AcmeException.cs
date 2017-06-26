using System;
using System.Net;

namespace THNETII.Acme.Client
{
    public class AcmeException : Exception
    {
        private const string defaultMessageString = "An unexpected error occurred during an ACME request.";

        public AcmeError Error { get; }

        public string Detail => Error?.Detail;

        public HttpStatusCode StatusCode => Error?.StatusCode ?? default(HttpStatusCode);

        public AcmeException() : base(defaultMessageString) { }
        public AcmeException(AcmeError error) : this(defaultMessageString, error) { }
        public AcmeException(string message, AcmeError error) : base(CreateFullMessageString(message, error))
        {
            Error = error;
        }
        public AcmeException(Exception innerException) : base(defaultMessageString, innerException) { }
        public AcmeException(AcmeError error, Exception innerException) : this(defaultMessageString, error, innerException) { }
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
