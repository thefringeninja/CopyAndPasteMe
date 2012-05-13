using System;

namespace Starbucks.Messages
{
    public class DrinkOrdered : Event
    {
        public Guid OrderId { get; set; }
        public Drinks Drink { get; set; }
        public decimal Price { get; set; }
    }
}