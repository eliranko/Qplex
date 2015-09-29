using System;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Communicator broadcasts and receives messages.
    /// </summary>
    public class Communicator : Broadcaster
    {
        /// <summary>
        /// Dispatcher
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Ctor
        /// </summary>
        public Communicator()
        {
            _dispatcher = new Dispatcher();
        }

        /// <summary>
        /// Start receiving thread
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            //TODO: Log
            //TODO: Iterate over message handlers and add them to the dispatcher
            throw new NotImplementedException();
        }

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New message</param>
        public void NewMessage(Message message)
        {
            //TODO: Log
            _dispatcher.HandleMessage(message);
        }
    }
}
