using System;

namespace Starbucks
{
    public interface IBus
    {
        /// <summary>
        /// Registers a delegate to handle this message
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="handler">The delegate</param>
        void Register<T>(Action<T> handler);

        /// <summary>
        /// Dispatches the message to all registered handlers
        /// </summary>
        /// <typeparam name="T">The type of message</typeparam>
        /// <param name="event">the message itself</param>
        void Publish<T>(T @event) where T : Event;

        /// <summary>
        /// Dispatches a message to a single handler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        void Send<T>(T command) where T : Command;
    }
}