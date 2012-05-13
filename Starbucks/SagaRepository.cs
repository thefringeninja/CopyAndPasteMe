using System;
using System.Collections.Generic;
using Starbucks.Actors;

namespace Starbucks
{
    public class SagaRepository<TSaga> : ISagaRepository<TSaga> where TSaga : class, ISaga
    {
        private static readonly IDictionary<Guid, List<Event>> storage = new Dictionary<Guid, List<Event>>();
        private readonly IBus bus;

        public SagaRepository(IBus bus)
        {
            this.bus = bus;
        }

        #region ISagaRepository<TSaga> Members

        public TSaga GetById(Guid id)
        {
            var saga = (TSaga) Activator.CreateInstance(typeof (TSaga), id);

            List<Event> events;
            if (false == storage.TryGetValue(id, out events))
            {
                return saga;
            }

            events.ForEach(saga.Transition);
            saga.MarkChangesAsCommittted();
            saga.MarkMessagesAsDispatched();
            return saga;
        }

        public void Save(TSaga saga)
        {
            List<Event> events;
            if (false == storage.TryGetValue(saga.Id, out events))
            {
                events = storage[saga.Id] = new List<Event>();
            }

            events.AddRange(saga.GetUncommittedChanges());
            saga.MarkChangesAsCommittted();

            saga.GetUndispatchedCommands().ForEach(bus.Send);
            saga.MarkMessagesAsDispatched();
        }

        #endregion
    }
}