using System;
using Qplex.Messages;
using Qplex.Messages.Handlers;

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
        /// <param name="messagesIteratorType">Messages iterator type</param>
        public Communicator(Type messagesIteratorType)
        {
            if (messagesIteratorType != typeof (IMessagesIterator))
            {
                Qplex.Instance.CloseApplication(
                    $"Message iterator type received does not impelements IMessagesIterator interface: {messagesIteratorType.FullName}");
            }

            _dispatcher = new Dispatcher(messagesIteratorType);
        }

        /// <summary>
        /// Start receiving thread
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            //TODO: Log
            //TODO: Iterate over message handlers and add them to the dispatcher
            _dispatcher.Start();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop dispatcher threads
        /// </summary>
        public void Stop()
        {
            _dispatcher.Stop();
        }

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New received message</param>
        public void NewMessage(Message message)
        {
            //TODO: Log
            _dispatcher.Dispatch(message);
        }
    }
}
