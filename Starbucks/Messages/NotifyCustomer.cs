using System;
using System.Runtime.Serialization;

namespace Starbucks.Messages
{
    [DataContract]
    public class NotifyCustomer : Command
    {
        [DataMember]
        public Guid OrderId { get; set; }
    }
}