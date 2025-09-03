// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TransactionQueue;
using OpenSharpTrace.Utilities;
using System;
using System.Net;

namespace OpenSharpTrace.Controllers
{
    /// <summary>
    /// OpenSharpTrace base controller. Traces will be persist every minute in a single transaction.
    /// </summary>
    public class OpenSharpTraceController : Controller
    {
        private readonly ITraceQueue<Trace> _transactionQueue;

        private readonly ILogger _logger;

        private const string ItemsRequestKey = "OST_REQUEST";
        private const string ItemsStartKey = "OST_EXEC_START";
        private const string TransactionKey = "TRANSACTION";
        private const string ConsumerKey = "CONSUMER";

        public OpenSharpTraceController(
            ILogger logger,
            ITraceQueue<Trace> transactionQueue)
        {
            _logger = logger;
            _transactionQueue = transactionQueue;
        }

        public OpenSharpTraceController(
            ILoggerFactory loggerFactory,
            ITraceQueue<Trace> transactionQueue)
        {
            _logger = loggerFactory.CreateLogger(GetType().ToString());
            _transactionQueue = transactionQueue;
        }

        /// <summary>
        /// OpenSharpTrace OnActionExecuting override.
        /// </summary>
        /// <param name="context">Context for action filters</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null) return;
            context.HttpContext.Items[ItemsRequestKey] = context.ActionArguments;
            context.HttpContext.Items[ItemsStartKey] = DateTime.UtcNow;

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// OpenSharpTrace OnActionExecuted override.
        /// </summary>
        /// <param name="context">Context for action filters</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context?.HttpContext;
            if (httpContext == null)
            {
                base.OnActionExecuted(context);
                return;
            }

            httpContext.Request.Headers.TryGetValue(TransactionKey, out var transaction);
            httpContext.Request.Headers.TryGetValue(ConsumerKey, out var client);

            var request = httpContext.Items[ItemsRequestKey];
            var startedAt = httpContext.Items[ItemsStartKey] as DateTime?;
            double? totalMs = startedAt.HasValue
                ? (DateTime.UtcNow - startedAt.Value).TotalMilliseconds
                : null;

            var result = context?.Result;
            int httpStatusCode =
                context?.Exception != null ? (int)HttpStatusCode.InternalServerError :
                (result as ObjectResult)?.StatusCode
                ?? (result as StatusCodeResult)?.StatusCode
                ?? (int)HttpStatusCode.OK;

            // write the current trace
            var trace = new Trace
            {
                TransactionId = transaction.ToString(),
                ClientId = client.ToString(),
                ServerId = httpContext.Request.Host.ToString(),
                HttpMethod = httpContext.Request.Method,
                HttpPath = httpContext.Request.Path,
                HttpStatusCode = httpStatusCode,
                ActionDescriptor = context?.ActionDescriptor?.DisplayName,
                RemoteAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
                JsonRequest = request.ToJson(),
                JsonResponse = (result as ObjectResult)?.Value.ToJson(),
                TimeStamp = DateTime.UtcNow,
                Exception = context?.Exception?.Message,
                ExecutionTime = totalMs
            };

            _transactionQueue.Enqueue(trace);

            base.OnActionExecuted(context);
        }
    }
}
