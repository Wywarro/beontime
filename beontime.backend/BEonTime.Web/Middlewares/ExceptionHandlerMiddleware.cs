using BEonTime.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BEonTime.Web.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        static ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        static async Task HandleExceptionAsync(
             HttpContext context, Exception exception)
        {
            _logger.LogError($"{{Time}} - {exception.Source} - {exception.Message} - " +
                $"{exception.StackTrace} - {exception.TargetSite.Name}", DateTime.UtcNow);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(new ErrorResponse()
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = "An internal server error has occured.",
                Content = exception.Message
            }.ToString());
        }
    }
}
