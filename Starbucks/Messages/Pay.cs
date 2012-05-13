using System;
using System.Runtime.Serialization;

namespace Starbucks.Messages
{
    [DataContract]
    public class Pay : Command
    {
        [DataMember]
        public Guid OrderId { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
}