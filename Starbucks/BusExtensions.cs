using System;
using System.Threading;

namespace Starbucks
{
    public static class BusExtensions
    {
        public static void RegisterOnThreadPool<T>(this IBus bus, Action<T> action)
        {
            bus.Register<T>(item => ThreadPool.QueueUserWorkItem(_ => action(item)));
        }

        public static void RouteToSaga<TSaga, TEvent>(this IBus bus, ISagaRepository<TSaga> repository,
                                                      Func<TEvent, Guid> correlatedBy) where TSaga : class, ISaga
            where TEvent : Event
        {
            bus.Register<TEvent>(e =>
                                     {
                                         Guid correlationId = correlatedBy(e);

                                         TSaga saga = repository.GetById(correlationId);
                                         saga.Transition(e);
                                         repository.Save(saga);
                                     });
        }
    }
}