// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using OpenSharpTrace.Persistence.SQL.Entities;
using System.Collections.Generic;

namespace OpenSharpTrace.Abstractions.Persistence
{
    public interface ISqlTraceRepository
    {
        void InsertMany(List<Trace> entity);
    }
}
