// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.Utilities;
using System;
using System.Linq;
using System.Net;

namespace OpenSharpTrace.Controllers
{
    /// <summary>
    /// OpenSharpTrace base controller
    /// </summary>
    public class OpenSharpTraceController : Controller
    {
        private readonly ISqlTraceRepository _repository;

        private readonly ILogger _logger;

        private const string RouteDataKey = "REQUEST";
        private const string TimeExecutionKey = "EXEC";
        private const string TransactionKey = "TRANSACTION";
        private const string ConsumerKey = "CONSUMER";

        public OpenSharpTraceController(ILogger logger, ISqlTraceRepository repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public OpenSharpTraceController(ILoggerFactory loggerFactory, ISqlTraceRepository repository)
        {
            _repository = repository;
            _logger = loggerFactory.CreateLogger(GetType().ToString());
        }

        /// <summary>
        /// OpenSharpTrace OnActionExecuting override.
        /// </summary>
        /// <param name="context">Context for action filters</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // add the request objects to RouteData
            context?.RouteData.Values.Add(RouteDataKey, context.ActionArguments);
            context?.RouteData.Values.Add(TimeExecutionKey, DateTime.UtcNow);

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// OpenSharpTrace OnActionExecuted override.
        /// </summary>
        /// <param name="context">Context for action filters</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var objectResult = context?.Result as ObjectResult;

            // get the header informations
            var transaction = context.HttpContext?.Request.Headers[TransactionKey].FirstOrDefault();
            var client = context.HttpContext?.Request.Headers[ConsumerKey].FirstOrDefault();

            // get the request objects from RouteData
            var request = context.RouteData.Values[RouteDataKey];
            var timeStamp = context.RouteData.Values[TimeExecutionKey];

            // read other usefull informations
            var totalExecutionTime = (DateTime.UtcNow - timeStamp.ToDateTime())?.TotalMilliseconds;
            var httpMethod = context.HttpContext?.Request.Method;
            var httpPath = context.HttpContext?.Request.Path;
            var actionDescriptor = context.ActionDescriptor?.DisplayName;
            var remoteAddress = Network.CleanNotationAddress(context.HttpContext?.Connection.RemoteIpAddress?.ToString());
            var host = context.HttpContext?.Request.Host.ToString();
            var exception = context.Exception?.Message;
            var httpStatusCode = objectResult?.StatusCode ?? (int)HttpStatusCode.OK;

            if (exception != null)
                httpStatusCode = (int)HttpStatusCode.InternalServerError;

            // get the object response
            var response = objectResult?.Value;

            // write the current trace
            var trace = new Trace()
            {
                TransactionId = transaction,
                ClientId = client,
                ServerId = host,
                HttpMethod = httpMethod,
                HttpPath = httpPath,
                HttpStatusCode = httpStatusCode,
                ActionDescriptor = actionDescriptor,
                RemoteAddress = remoteAddress,
                JsonRequest = request.ToJson(),
                JsonResponse = response.ToJson(),
                TimeStamp = DateTime.UtcNow,
                Exception = exception,
                ExecutionTime = totalExecutionTime
            };
            _repository.Insert(trace);

            base.OnActionExecuted(context);
        }
    }
}
