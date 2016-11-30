using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Ontap.Auth
{
    public class JsonErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;

        public JsonErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var processed = await HandleExceptionAsync(context, ex);
                if (!processed)
                {
                    throw;
                }
            }
        }

        private async Task<bool> HandleExceptionAsync(HttpContext context, Exception exception)
        {
            if (exception == null || context?.Request?.ContentType?.Contains("json") != true)
                return false;

            var code = HttpStatusCode.InternalServerError;

            if (exception is AlreadyExistsException) code = HttpStatusCode.Conflict;
            else if (exception is KeyNotFoundException) code = HttpStatusCode.NotFound;
            else if (exception is InvalidCredentialException) code = HttpStatusCode.Forbidden;

            await WriteExceptionAsync(context, exception, code).ConfigureAwait(false);

            _loggerFactory.CreateLogger("Json").LogError(new EventId(), exception, "Business error");

            return true;
        }

        private static async Task WriteExceptionAsync(HttpContext context, Exception exception, HttpStatusCode code)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)code;
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                error = new
                {
                    message = exception.Message,
                    exception = exception.GetType().Name
                }
            })).ConfigureAwait(false);
        }
    }

    public class AlreadyExistsException : ArgumentException
    {
        public AlreadyExistsException(string message) : base(message)
        {
        }
    }
}
