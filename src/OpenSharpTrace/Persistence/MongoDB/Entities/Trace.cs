// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OpenSharpTrace.Persistence.MongoDB.Entities
{
    [Serializable]
    [BsonIgnoreExtraElements]
    public class Trace
    {
        //[BsonId]
        //public ObjectId Id { get; set; }
        [BsonElement("TransactionId")]
        public string? TransactionId { get; set; }
        [BsonElement("ServerId")]
        public string? ServerId { get; set; }
        [BsonElement("ClientId")]
        public string? ClientId { get; set; }
        [BsonElement("HttpMethod")]
        public string? HttpMethod { get; set; }
        [BsonElement("HttpPath")]
        public string? HttpPath { get; set; }
        [BsonElement("HttpStatusCode")]
        public int? HttpStatusCode { get; set; }
        [BsonElement("RemoteAddress")]
        public string? RemoteAddress { get; set; }
        [BsonElement("JsonRequest")]
        public string? JsonRequest { get; set; }
        [BsonElement("JsonResponse")]
        public string? JsonResponse { get; set; }
        [BsonElement("TimeStamp")]
        public DateTime? TimeStamp { get; set; }
        [BsonElement("Exception")]
        public string? Exception { get; set; }

        public Trace()
        {
            //Id = ObjectId.GenerateNewId();
        }
    }
}
