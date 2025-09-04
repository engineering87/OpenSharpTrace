// (c) 2022-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.SQL.Entities;
using OpenSharpTrace.TransactionQueue;
using OpenSharpTrace.TransactionScheduler;
using System.Linq;

namespace OpenSharpTrace.Test
{
    internal sealed class SeededQueue : ITraceQueue<Trace>
    {
        private readonly Queue<Trace> _q = new();
        public SeededQueue(IEnumerable<Trace> seed)
        {
            foreach (var t in seed) _q.Enqueue(t);
        }
        public void Enqueue(Trace item) => _q.Enqueue(item);
        public bool TryDequeue(out Trace item)
        {
            if (_q.Count > 0) { item = _q.Dequeue(); return true; }
            item = null; return false;
        }
        public int Count() => _q.Count;
    }

    [TestFixture]
    public class ServiceTransactionTests
    {
        [Test]
        public async Task WriteTraceFromQueueAsync_DequeuesAndCallsRepository()
        {
            // Arrange
            var traces = new[]
            {
                new Trace { TransactionId = "t1" },
                new Trace { TransactionId = "t2" }
            };
            var queue = new SeededQueue(traces);

            var repoMock = new Mock<ISqlTraceRepository>(MockBehavior.Strict);
            repoMock
                .Setup(r => r.InsertManyAsync(It.IsAny<IEnumerable<Trace>>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var services = new ServiceCollection()
                .AddSingleton(repoMock.Object)
                .BuildServiceProvider();

            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            var svc = new ServiceTransaction(queue, scopeFactory);

            // Act
            await svc.WriteTraceFromQueueAsync();

            // Assert
            repoMock.Verify(r => r.InsertManyAsync(It.Is<IEnumerable<Trace>>(l =>
                l != null &&
                l.Count() == 2 &&
                l.First().TransactionId == "t1" &&
                l.Last().TransactionId == "t2"
            )), Times.Once);

            Assert.That(queue.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task WriteTraceFromQueueAsync_EmptyQueue_DoesNothing()
        {
            var queue = new SeededQueue(System.Array.Empty<Trace>());
            var repoMock = new Mock<ISqlTraceRepository>(MockBehavior.Strict);

            var services = new ServiceCollection()
                .AddSingleton(repoMock.Object)
                .BuildServiceProvider();

            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
            var svc = new ServiceTransaction(queue, scopeFactory);

            await svc.WriteTraceFromQueueAsync();

            repoMock.VerifyNoOtherCalls();
            Assert.That(queue.Count(), Is.EqualTo(0));
        }
    }
}