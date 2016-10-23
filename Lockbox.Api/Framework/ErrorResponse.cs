using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Nancy;
using Nancy.Configuration;
using Nancy.ErrorHandling;
using Nancy.Responses;
using NLog;
using System.Linq;

namespace Lockbox.Api.Framework
{
    public class ErrorResponse : JsonResponse
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ErrorResponse(ErrorMessage error, INancyEnvironment environment)
            : base(error, new DefaultJsonSerializer(environment), environment)
        {
        }

        public static ErrorResponse FromException(Exception exception, INancyEnvironment environment)
        {
            Logger.Error(exception);

            var statusCode = HttpStatusCode.InternalServerError;
            if (exception is ArgumentNullException)
                statusCode = HttpStatusCode.BadRequest;
            else if (exception is AuthenticationException)
                statusCode = HttpStatusCode.Unauthorized;
            else if (exception is RouteExecutionEarlyExitException)
                statusCode = HttpStatusCode.Unauthorized;
            else if (exception is ArgumentException)
                statusCode = HttpStatusCode.BadRequest;
            else if (exception is InvalidOperationException)
                statusCode = HttpStatusCode.BadRequest;
            else if (exception is NullReferenceException)
                statusCode = HttpStatusCode.BadRequest;


            var error = ErrorMessage.FromExceptions(exception);
            var response = new ErrorResponse(error, environment) {StatusCode = statusCode};
            return response;
        }

        private class ErrorMessage
        {
            public readonly IEnumerable<ErrorDto> Errors;

            private ErrorMessage(IEnumerable<ErrorDto> errors)
            {
                Errors = errors;
            }

            public static ErrorMessage FromExceptions(params Exception[] exceptions)
                => new ErrorMessage(exceptions.Select(CreateErrorDto));

            private static ErrorDto CreateErrorDto(Exception exception)
                => new ErrorDto
                {
                    Message = exception.Message
                };
        }

        private class ErrorDto
        {
            public string Message { get; set; }
        }
    }
}