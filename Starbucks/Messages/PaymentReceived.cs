using System;

namespace Starbucks.Messages
{
    public class PaymentReceived : Event
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}