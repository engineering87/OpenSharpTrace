// (c) 2022 Francesco Del Re <francesco.delre.87@gmail.com>
// This code is licensed under MIT license (see LICENSE.txt for details)
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenSharpTrace.Persistence.SQL.Entities
{
    [Table("Trace")]
    public class Trace
    {
        [ScaffoldColumn(false)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string TransactionId { get; set; }
        public string ServerId { get; set; }
        public string ClientId { get; set; }
        public string HttpMethod { get; set; }
        public string HttpPath { get; set; }
        public int? HttpStatusCode { get; set; }
        public string ActionDescriptor { get; set; }
        public string RemoteAddress { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public double? ExecutionTime { get; set; }

        public Trace()
        {
            // empty constructor
        }
    }
}
