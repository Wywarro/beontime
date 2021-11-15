
using Beontime.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Beontime.WebApi.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleware> logger;

        public ExceptionHandlerMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(
             HttpContext context, Exception exception)
        {
            logger.LogError($"" +
                "{Time} - " +
                "{Source} - " +
                "{Message} - " +
                "{StackTrace} - " +
                "{TargetSiteName}",
                    DateTime.UtcNow,
                    exception.Source,
                    exception.Message,
                    exception.StackTrace,
                    exception.TargetSite?.Name);

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
