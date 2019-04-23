using System;
using System.Net;
using Balance.Infrastructure.Exceptions;
using Balance.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Balance.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var (error, statusCode) = PrepareResponseForException(context.Exception);

            context.ExceptionHandled = true;

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int)statusCode
            };
        }

        private (ResponseError, HttpStatusCode) PrepareResponseForException(Exception exception)
        {
            ResponseError error;
            HttpStatusCode statusCode;

            switch (exception)
            {              
                case InvalidDataException dataEx:
                    statusCode = HttpStatusCode.BadRequest;
                    error = new ResponseError(dataEx.Message);
                    break;
                case InvalidPermissionException permissionException:
                    statusCode = HttpStatusCode.BadRequest;
                    error = new ResponseError(permissionException.Message);
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    error = new ResponseError
                    {
                        Message = "Internal Server Error"
                    };

                    _logger?.LogCritical(exception, $"REST API Internal Server Error: {exception.Message}");

                    break;
            }

            return (error, statusCode);
        }
    }
}