// (c) 2022-2025 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using NUnit.Framework;
using OpenSharpTrace.TransactionQueue;

namespace OpenSharpTrace.Test
{
    [TestFixture]
    public class TraceQueueTests
    {
        [Test]
        public void Enqueue_Then_TryDequeue_ReturnsSameItem()
        {
            var q = new TraceQueue<int>();
            q.Enqueue(42);

            var ok = q.TryDequeue(out var value);

            Assert.That(ok, Is.True);
            Assert.That(value, Is.EqualTo(42));
            Assert.That(q.Count(), Is.EqualTo(0));
        }

        [Test]
        public void TryDequeue_OnEmpty_ReturnsFalseAndNull()
        {
            var q = new TraceQueue<string>();

            var ok = q.TryDequeue(out var value);

            Assert.That(ok, Is.False);
            Assert.That(value, Is.Null);
        }
    }
}