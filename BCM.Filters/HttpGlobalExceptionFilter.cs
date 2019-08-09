using System.Net;
using BCM.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BCM.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        private readonly IHostingEnvironment _env;
        public HttpGlobalExceptionFilter(ILoggerFactory loggerFactory, ILogger<HttpGlobalExceptionFilter> logger, IHostingEnvironment env)
        {
            // loggerFactory.AddNLog();
            // _logger = logger;
            _logger = loggerFactory.CreateLogger<HttpGlobalExceptionFilter>();
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);
            if (_env.IsDevelopment())
            {
                context.Result = new ApplicationErrorResult(Message.ServerError().Add("excetion", context.Exception));
            }
            else
            {
                context.Result = new ApplicationErrorResult(Message.ServerError().Add("excetion", new { context.Exception.Message, context.Exception.Source }));
            }

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
        }
        public class ApplicationErrorResult : ObjectResult
        {
            public ApplicationErrorResult(object value) : base(value)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}