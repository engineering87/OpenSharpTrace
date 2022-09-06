// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using OpenSharpTrace.Persistence.MongoDB.Entities;

namespace OpenSharpTrace.Abstractions.Persistence
{
    public interface IMongoDBTraceRepository
    {
        void Insert(Trace entity);
    }
}
