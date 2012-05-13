using System;
using System.Collections.Generic;
using Starbucks.Infrastructure;

namespace Starbucks
{
    //FROM http://blogs.msdn.com/b/davidebb/archive/2010/01/18/use-c-4-0-dynamic-to-drastically-simplify-your-private-reflection-code.aspx
    //doesnt count to line counts :)


    public class Bus : IBus
    {
        private readonly IDictionary<Type, IList<Action<object>>> handlersByType =
            new Dictionary<Type, IList<Action<Object>>>();

        #region IBus Members

        public void Publish<T>(T @event) where T : Event
        {
            IList<Action<object>> handlers;
            if (false == handlersByType.TryGetValue(@event.GetType(), out handlers))
            {
                return;
            }
            foreach (var handler in handlers)
            {
                handler(@event);
            }
        }

        public void Send<T>(T command) where T : Command
        {
            IList<Action<object>> handlers;
            if (!handlersByType.TryGetValue(command.GetType(), out handlers))
            {
                throw new InvalidOperationException("no handler registered");
            }
            if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
            handlers[0](command);
        }

        public void Register<T>(Action<T> handler)
        {
            IList<Action<object>> handlers;
            if (false == handlersByType.TryGetValue(typeof (T), out handlers))
            {
                handlersByType[typeof (T)] = handlers = new List<Action<Object>>();
            }

            handlers.Add(DelegateAdjuster.CastArgument<object, T>(x => handler(x)));
        }

        #endregion
    }
}