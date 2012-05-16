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

        protected void DispatchWhen<T>(IObservable<T> observable, Func<T, Command> command, int numberOfTimes = 1)
        {
            if (numberOfTimes <= 0)
            {
                throw new InvalidOperationException();
            }
            observable.Take(numberOfTimes).Select(command).Subscribe(undispatched.Add);
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