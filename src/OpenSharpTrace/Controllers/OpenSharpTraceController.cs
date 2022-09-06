// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;

namespace OpenSharpTrace.Controllers
{
    public class OpenSharpTraceController : Controller
    {
        private readonly ISqlTraceRepository _repository;

        private readonly ILogger _logger;

        private const string RouteDataKey = "REQUEST";
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

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// OpenSharpTrace OnActionExecuted override.
        /// </summary>
        /// <param name="context">Context for action filters</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context?.Result is ObjectResult objectResult)
            {
                // get the header informations
                var transaction = context.HttpContext?.Request.Headers[TransactionKey].FirstOrDefault();
                var client = context.HttpContext?.Request.Headers[ConsumerKey].FirstOrDefault();

                // get the request objects from RouteData
                var request = context.RouteData.Values[RouteDataKey];

                // read other usefull informations
                var httpMethod = context.HttpContext?.Request.Method;
                var httpPath = context.HttpContext?.Request.Path;
                var httpStatusCode = objectResult.StatusCode;
                var remoteAddress = context.HttpContext?.Connection.RemoteIpAddress?.ToString();
                var host = context.HttpContext?.Request.Host.ToString();
                var exception = context.Exception?.Message;

                // get the object response
                var response = objectResult.Value;

                // write the current trace
                var trace = new Trace()
                {
                    TransactionId = transaction,
                    ClientId = client,
                    ServerId = host,
                    HttpMethod = httpMethod,
                    HttpPath = httpPath,
                    HttpStatusCode = httpStatusCode,
                    RemoteAddress = remoteAddress,
                    JsonRequest = JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    JsonResponse = JsonConvert.SerializeObject(response, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    TimeStamp = DateTime.UtcNow,
                    Exception = exception
                };
                _repository.Insert(trace);
            }

            base.OnActionExecuted(context);
        }
    }
}
