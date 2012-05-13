using System;
using System.Threading;
using Starbucks.Actors;
using Starbucks.Messages;

namespace Starbucks
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var bus = new Bus();

            var cashier = new Cashier(bus);
            var barista = new Barista(Guid.NewGuid(), bus);

            var repository = new SagaRepository<OrderFufillment>(bus);
            bus.RouteToSaga(repository, (DrinkOrdered e) => e.OrderId);
            bus.RouteToSaga(repository, (DrinkPrepared e) => e.OrderId);
            bus.RouteToSaga(repository, (PaymentReceived e) => e.OrderId);


            bus.Register<OrderDrink>(c => cashier.Order(c.OrderId, c.Drink));
            bus.RegisterOnThreadPool<Pay>(c => cashier.Pay(c.OrderId, PaymentMethod.CreditCard, c.Amount));
            bus.RegisterOnThreadPool<PrepareDrink>(c => barista.PrepareDrink(c.OrderId, c.Drink));
            bus.Register<NotifyCustomer>(c =>
                                             {
                                                 Console.WriteLine("{0} is ready", c.OrderId);
                                                 ThreadPool.QueueUserWorkItem(_ => bus.Send(new Pay
                                                                                                {
                                                                                                    OrderId =
                                                                                                        c.OrderId,
                                                                                                    Amount = 12m
                                                                                                }));
                                             });


            Guid orderId = Guid.NewGuid();
            bus.Send(new OrderDrink {Drink = Drinks.Cappucino, OrderId = orderId});
            bus.Send(new Pay
                         {
                             OrderId = orderId,
                             Amount = 10m
                         });

            Console.ReadLine();
        }
    }
}