// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using OpenSharpTrace.Persistence.SQL;
using OpenSharpTrace.Persistence.SQL.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace OpenSharpTrace.Test.SQL
{
    public class TraceTests
    {
        [SetUp]
        public void Setup()
        {
            var db = GetMemoryContext();
            db.Database.EnsureDeleted();
            db.Trace.Add(new Trace
            {
                HttpPath = "/test",
                HttpStatusCode = (int)HttpStatusCode.OK,
                HttpMethod = "Test",
                RemoteAddress = "127.0.0.1",
                JsonRequest = "{}",
                JsonResponse = "{}",
                TimeStamp = DateTime.UtcNow,
                ActionDescriptor = "test_action_descriptor",
                Exception = string.Empty
            });
            db.SaveChanges();
        }

        [Test]
        public void CanAddManyTrace()
        {
            var db = GetMemoryContext();
            var traceFirst = new Trace
            {
                HttpPath = "/AddManyTrace",
                HttpStatusCode = (int)HttpStatusCode.OK,
                HttpMethod = "Test1",
                RemoteAddress = "127.0.0.1",
                JsonRequest = "{}",
                JsonResponse = "{}",
                TimeStamp = DateTime.UtcNow,
                ActionDescriptor = "test_action_descriptor1",
                Exception = string.Empty
            };
            var traceSecond = new Trace
            {
                HttpPath = "/AddManyTrace",
                HttpStatusCode = (int)HttpStatusCode.OK,
                HttpMethod = "Test2",
                RemoteAddress = "127.0.0.1",
                JsonRequest = "{}",
                JsonResponse = "{}",
                TimeStamp = DateTime.UtcNow,
                ActionDescriptor = "test_action_descriptor2",
                Exception = string.Empty
            };
            var processor = new SqlTraceRepository(NullLoggerFactory.Instance, db);

            Assert.DoesNotThrow(() => processor.InsertMany(new List<Trace> { traceFirst, traceSecond }));
        }

        public static TraceContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TraceContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options; return new TraceContext(options);
        }
    }
}
