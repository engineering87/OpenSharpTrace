using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSharpTrace.Persistence.MongoDB
{
    public class TraceContext
    {
        private IMongoDatabase MongoDatabase { get; set; }
        private MongoClient MongoClient { get; set; }
        public IClientSessionHandle Session { get; set; }

        //public TraceContext(IOptions configuration)
        //{
        //    MongoClient = new MongoClient(configuration.Value.Connection);
        //    MongoDatabase = MongoClient.GetDatabase(configuration.Value.DatabaseName);
        //}
    }
}
