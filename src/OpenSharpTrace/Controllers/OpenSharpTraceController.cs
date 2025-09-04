// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
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
    /// OpenSharpTrace base controller. Traces will be persisted every minute in a single transaction.
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

            DateTime? startedAt = null;
            if (httpContext.Items.TryGetValue(ItemsStartKey, out var startedObj) &&
                startedObj is DateTime started)
            {
                startedAt = started;
            }

            double? totalMs = startedAt.HasValue
                ? (DateTime.UtcNow - startedAt.Value).TotalMilliseconds
                : null;

            var result = context?.Result;
            int httpStatusCode =
                context?.Exception != null ? (int)HttpStatusCode.InternalServerError :
                (result as ObjectResult)?.StatusCode
                ?? (result as StatusCodeResult)?.StatusCode
                ?? (httpContext.Response?.StatusCode ?? (int)HttpStatusCode.OK);

            var remote = httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !string.IsNullOrWhiteSpace(xff)
                ? xff.ToString().Split(',')[0].Trim()
                : httpContext.Connection.RemoteIpAddress?.ToString();

            var path = httpContext.Request.Path.ToString();
            var query = httpContext.Request.QueryString.HasValue
                ? httpContext.Request.QueryString.Value
                : string.Empty;

            // write the current trace
            var trace = new Trace
            {
                TransactionId = transaction.ToString(),
                ClientId = client.ToString(),
                ServerId = httpContext.Request.Host.ToString(),
                HttpMethod = httpContext.Request.Method,
                HttpPath = $"{path}{query}",
                HttpStatusCode = httpStatusCode,
                ActionDescriptor = context?.ActionDescriptor?.DisplayName,
                RemoteAddress = Network.CleanNotationAddress(remote),
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
