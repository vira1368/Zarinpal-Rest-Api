using System;
using System.Net;

namespace ZarinpalRestApi.Helpers
{
    public class ZarinpalException : Exception
    {
        public HttpStatusCode? HttpStatusCode { get; private set; }

        public ZarinpalException()
        {
        }

        public ZarinpalException(HttpStatusCode? httpStatusCode, string? message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ZarinpalException(HttpStatusCode? httpStatusCode, string? message, Exception? innerException) : base(message,
            innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public ZarinpalException(string? message) : this(null, message)
        {
        }

        public ZarinpalException(string? message, Exception? innerException) : this(null, message, innerException)
        {
        }
    }
}