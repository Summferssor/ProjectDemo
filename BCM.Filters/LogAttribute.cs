using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BCM.Filters
{
    public class LogAttribute : ActionFilterAttribute
    {
        private readonly ILogger<LogAttribute> _logger;
        private string ActionArguments { get; set; }
        private Stopwatch Stopwatch { get; set; }

        public LogAttribute(ILoggerFactory loggerFactory, ILogger<LogAttribute> logger)
        {
            _logger = loggerFactory.CreateLogger<LogAttribute>();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            Stopwatch.Stop();

            string url = context.HttpContext.Request.Host + context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            string method = context.HttpContext.Request.Method;

            string qs = ActionArguments;
            dynamic result = null;
            if(context.Result != null)
            {
                result = context.Result.GetType().Name == "emptyResult" ? new { Value = "无返回结果" } : context.Result as dynamic;
            }
            string res = "在返回结果前发生了异常";
            try
            {
                if (result != null)
                {
                    res = Newtonsoft.Json.JsonConvert.SerializeObject(result.Value);
                }
            }
            catch (System.Exception)
            {
                res = "日志未获取到结果，返回的数据无法序列化";
            }


            _logger.LogInformation($"\n 方法：123 \n " +
                $"地址：{url} \n " +
                $"方式：{method} \n " +
                $"参数：{qs}\n " +
                $"结果：{res}\n " +
                $"耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ActionArguments = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);

            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }
    }
}
