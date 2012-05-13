using System;
using System.Runtime.Serialization;

namespace Starbucks.Messages
{
    [DataContract]
    public class OrderDrink : Command
    {
        [DataMember]
        public Guid OrderId { get; set; }

        [DataMember]
        public Drinks Drink { get; set; }
    }
}