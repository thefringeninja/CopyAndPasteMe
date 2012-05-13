using System;
using System.Collections.Generic;
using System.Threading;
using Starbucks.Messages;

namespace Starbucks.Actors
{
    public class Cashier
    {
        private static readonly IDictionary<Drinks, decimal> prices
            = new Dictionary<Drinks, decimal>
                  {
                      {Drinks.Espresso, 4.99m},
                      {Drinks.Cappucino, 8.99m},
                      {Drinks.Frappacino, 3.99m}
                  };

        private readonly IBus bus;

        private readonly IList<Guid> paid = new List<Guid>();

        public Cashier(IBus bus)
        {
            this.bus = bus;
        }

        public void Order(Guid orderId, Drinks drink)
        {
            Console.WriteLine("[Thread {2}] Order up! One {0} for receipt {1}", drink, orderId,
                              Thread.CurrentThread.ManagedThreadId);
            bus.Publish(new DrinkOrdered
                            {
                                Drink = drink,
                                OrderId = orderId,
                                Price = prices[drink]
                            });
        }

        public void Pay(Guid orderId, PaymentMethod methodOfPayment, decimal amount)
        {
            if (paid.Contains(orderId))
            {
                Console.WriteLine("Free money awesome");
            }

            if (methodOfPayment == PaymentMethod.Check)
            {
                throw new InvalidOperationException("[Thread {0}] We don't take checks");
            }

            if (methodOfPayment == PaymentMethod.CreditCard)
            {
                Console.WriteLine("[Thread {0}] Please enter your pin", Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(5000);
                Console.WriteLine("[Thread {0}] Hit OK", Thread.CurrentThread.ManagedThreadId);
            }

            Console.WriteLine("[Thread {0}] Thank you!", Thread.CurrentThread.ManagedThreadId);

            paid.Add(orderId);
            bus.Publish(new PaymentReceived
                            {
                                OrderId = orderId,
                                Amount = amount
                            });
        }
    }
}