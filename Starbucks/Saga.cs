using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Starbucks
{
    public class Saga<TSaga> : ISaga
        where TSaga : Saga<TSaga>, ISaga
    {
        public static IDictionary<Type, Func<Object, Guid>> correlations;
        private readonly IList<Event> changes = new List<Event>();
        private readonly ISubject<Event> source;
        private readonly IList<Command> undispatched = new List<Command>();
        private bool completed;

        public Saga(Guid id)
        {
            Id = id;
            source = new Subject<Event>();
            source.Subscribe(changes.Add);
            source.OfType<SagaCompleted>().Subscribe(_ => { completed = true; });
        }

        #region ISaga Members

        public Guid Id { get; private set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return changes.AsEnumerable();
        }

        public IEnumerable<Command> GetUndispatchedCommands()
        {
            return undispatched.AsEnumerable();
        }

        public void MarkChangesAsCommittted()
        {
            changes.Clear();
        }

        public void MarkMessagesAsDispatched()
        {
            undispatched.Clear();
        }

        public void Transition<TEvent>(TEvent @event) where TEvent : Event
        {
            if (completed)
            {
                Console.WriteLine("Sorry guy we're done");
                return;
            }
            source.OnNext(@event);
        }

        #endregion

        protected void Dispatch(Command command)
        {
            undispatched.Add(command);
        }

        protected IObservable<TEvent> Subscribe<TEvent>()
        {
            return source.OfType<TEvent>();
        }

        protected void Complete()
        {
            Transition(new SagaCompleted());
            source.OnCompleted();
        }
    }
}