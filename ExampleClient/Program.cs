using System;
using System.Threading;
using Fizum.Tests.ServiceReference1;

namespace Fizum.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var client = new SalesClient())
            {
                Guid orderId = Guid.NewGuid();
                client.OrderDrink(new OrderDrink
                                      {
                                          OrderId = orderId,
                                          Drink = Drinks.Cappucino
                                      });
                Thread.Sleep(5000);
                client.Pay(new Pay
                               {
                                   OrderId = orderId,
                                   Amount = 12.95m
                               });
            }
        }
    }
}