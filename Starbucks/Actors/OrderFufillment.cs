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

            orders.Take(1)
                .Select(e => new PrepareDrink
                                 {
                                     OrderId = e.OrderId,
                                     Drink = e.Drink
                                 }).Subscribe(Dispatch);

            payments.Zip(preparations, Tuple.Create)
                .Take(1)
                .Select(result => new NotifyCustomer
                                      {
                                          OrderId = result.Item1.OrderId
                                      }).Subscribe(Dispatch, Complete);
        }
    }
}