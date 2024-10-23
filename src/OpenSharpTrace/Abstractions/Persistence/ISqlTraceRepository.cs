// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using OpenSharpTrace.Persistence.SQL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSharpTrace.Abstractions.Persistence
{
    public interface ISqlTraceRepository
    {
        Task InsertManyAsync(List<Trace> entity);
    }
}
