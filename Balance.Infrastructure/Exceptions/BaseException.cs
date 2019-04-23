using System;
using System.Net;

namespace Balance.Infrastructure.Exceptions
{
    public class BaseException : Exception
    {
        protected int StatusCode { get; set; } = 400;

        public BaseException()
        { }

        protected BaseException(string message)
            : base(message)
        { }

        public BaseException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = (int)statusCode;
        }

        public BaseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}