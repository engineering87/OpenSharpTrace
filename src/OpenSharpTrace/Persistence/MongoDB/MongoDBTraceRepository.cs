// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using OpenSharpTrace.Abstractions.Persistence;
using OpenSharpTrace.Persistence.MongoDB.Entities;

namespace OpenSharpTrace.Persistence.MongoDB
{
    public class MongoDBTraceRepository : IMongoDBTraceRepository, IRepository<Trace>
    {
        public MongoDBTraceRepository(MongoDBConfig mongoDBConfig)
        {
            // TODO
        }

        public void Insert(Trace entity)
        {
            throw new NotImplementedException();
        }
    }
}
