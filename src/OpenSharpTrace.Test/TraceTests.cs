// (c) 2022-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using OpenSharpTrace.Controllers;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TransactionQueue;

namespace OpenSharpTrace.Test
{
    // Simple in-memory queue to assert what the controller enqueues
    internal sealed class InMemoryTraceQueue : ITraceQueue<Trace>
    {
        private readonly Queue<Trace> _items = new();
        public void Enqueue(Trace item) => _items.Enqueue(item);
        public bool TryDequeue(out Trace item)
        {
            if (_items.Count > 0) { item = _items.Dequeue(); return true; }
            item = null; return false;
        }
        public int Count() => _items.Count;
        public Trace PeekOrDefault() => _items.Count > 0 ? _items.Peek() : null;
    }

    [TestFixture]
    public class ControllerTests
    {
        private static (ActionExecutingContext exec, ActionExecutedContext done, DefaultHttpContext http)
            BuildContexts(Controller controller)
        {
            var http = new DefaultHttpContext();
            http.Request.Method = "GET";
            http.Request.Scheme = "https";
            http.Request.Host = new HostString("api.example.test");
            http.Request.Path = "/test/endpoint";
            http.Request.QueryString = new QueryString("?q=abc");
            http.Connection.RemoteIpAddress = IPAddress.Parse("::ffff:10.0.0.42"); // IPv6-mapped
            http.Request.Headers["TRANSACTION"] = "tx-123";
            http.Request.Headers["CONSUMER"] = "client-xyz";
            http.Request.Headers["X-Forwarded-For"] = "::ffff:192.168.1.10";

            var route = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor
            {
                ControllerName = "Test",
                ActionName = "Run",
                DisplayName = "TestController.Run"
            };
            var actionContext = new ActionContext(http, route, actionDescriptor);

            var filters = new List<IFilterMetadata>();
            var args = new Dictionary<string, object> { { "id", 7 } };

            var executing = new ActionExecutingContext(actionContext, filters, args, controller);
            var executed = new ActionExecutedContext(actionContext, filters, controller)
            {
                Result = new ObjectResult(new { ok = true }) { StatusCode = (int)HttpStatusCode.Accepted }
            };

            return (executing, executed, http);
        }

        [Test]
        public void OnAction_PopulatesTrace_And_Enqueues()
        {
            // Arrange
            var queue = new InMemoryTraceQueue();
            var controller = new OpenSharpTraceController(NullLogger.Instance, queue);
            var (executing, executed, http) = BuildContexts(controller);

            // Act
            controller.OnActionExecuting(executing);
            System.Threading.Thread.Sleep(5); // simulate work
            controller.OnActionExecuted(executed);

            // Assert
            var trace = queue.PeekOrDefault();
            Assert.That(trace, Is.Not.Null);
            Assert.That(trace.TransactionId, Is.EqualTo("tx-123"));
            Assert.That(trace.ClientId, Is.EqualTo("client-xyz"));
            Assert.That(trace.HttpMethod, Is.EqualTo("GET"));
            Assert.That(trace.HttpPath, Is.EqualTo("/test/endpoint?q=abc"));
            Assert.That(trace.HttpStatusCode, Is.EqualTo((int)HttpStatusCode.Accepted));
            Assert.That(trace.ActionDescriptor, Is.EqualTo("TestController.Run"));
            Assert.That(trace.RemoteAddress, Is.EqualTo("192.168.1.10")); // XFF + IPv6-mapped cleaned
            Assert.That(trace.JsonRequest, Is.Not.Null);
            Assert.That(trace.JsonResponse, Is.Not.Null);
            Assert.That(trace.ExecutionTime, Is.GreaterThan(0));
            Assert.That(trace.ServerId, Is.EqualTo(http.Request.Host.ToString()));
        }
    }
}