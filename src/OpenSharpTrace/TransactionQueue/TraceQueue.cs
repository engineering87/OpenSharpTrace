// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System.Collections.Concurrent;

namespace OpenSharpTrace.TransactionQueue
{
    /// <summary>
    /// Global queue for track persistence.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TraceQueue<T> : ITraceQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new();

        public void Enqueue(T item) => _queue.Enqueue(item);

        public bool TryDequeue(out T item) => _queue.TryDequeue(out item);

        public int Count() => _queue.Count;
    }
}
