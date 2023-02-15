// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
namespace OpenSharpTrace.TraceQueue
{
    public interface ITraceQueue<T>
    {
        void Enqueue(T item);
        T Dequeue();
        int Count();
    }
}
