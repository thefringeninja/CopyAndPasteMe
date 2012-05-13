using System;
using System.Collections.Generic;

namespace Starbucks
{
    public interface ISaga
    {
        Guid Id { get; }
        IEnumerable<Event> GetUncommittedChanges();
        IEnumerable<Command> GetUndispatchedCommands();

        void MarkChangesAsCommittted();
        void MarkMessagesAsDispatched();

        void Transition<TEvent>(TEvent @event) where TEvent : Event;
    }
}