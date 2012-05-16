using System;
using System.Reactive.Linq;
using Starbucks.Messages;

namespace Starbucks.Actors
{
    public class OrderFufillment : Saga<OrderFufillment>
    {
        public OrderFufillment(Guid id)
            : base(id)
        {
            IObservable<DrinkOrdered> orders = Subscribe<DrinkOrdered>();
            IObservable<PaymentReceived> payments = Subscribe<PaymentReceived>();
            IObservable<DrinkPrepared> preparations = Subscribe<DrinkPrepared>();

            DispatchWhen(orders, e => new PrepareDrink
                                          {
                                              OrderId = e.OrderId,
                                              Drink = e.Drink
                                          });

            DispatchWhen(payments.Zip(preparations, Tuple.Create),
                         result => new NotifyCustomer
                                       {
                                           OrderId = result.Item1.OrderId
                                       });
        }
    }
}