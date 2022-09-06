// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using OpenSharpTrace.Persistence.SQL.Entities;

namespace OpenSharpTrace.Abstractions.Persistence
{
    public interface ISqlTraceRepository
    {
        void Insert(Trace entity);
    }
}
