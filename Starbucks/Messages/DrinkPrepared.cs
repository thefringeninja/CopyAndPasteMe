using System;

namespace Starbucks.Messages
{
    public class DrinkPrepared : Event
    {
        public Guid OrderId { get; set; }
        public Drinks Drink { get; set; }
        public Guid BaristaId { get; set; }
    }
}