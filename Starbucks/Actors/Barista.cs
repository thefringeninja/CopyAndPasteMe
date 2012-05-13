using System;
using System.Collections.Generic;
using System.Threading;
using Starbucks.Messages;

namespace Starbucks.Actors
{
    public class Barista
    {
        private readonly IBus bus;
        private readonly Guid id;
        private readonly IList<Guid> recentOrders = new List<Guid>();

        public Barista(Guid id, IBus bus)
        {
            this.id = id;
            this.bus = bus;
        }

        public void PrepareDrink(Guid orderId, Drinks drink)
        {
            if (recentOrders.Contains(orderId))
            {
                Console.WriteLine("Already prepared this wtf");
                return;
            }
            Console.WriteLine("[Thread {1}] Preparing {0}", drink, Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(((int) drink + 1)*1000);
            Console.WriteLine("[Thread {1}] Done preparing {0}", drink, Thread.CurrentThread.ManagedThreadId);
            recentOrders.Add(orderId);
            bus.Publish(new DrinkPrepared
                            {
                                OrderId = orderId,
                                BaristaId = id,
                                Drink = drink
                            });
        }
    }
}