using System;
using System.Collections.Generic;
using System.Threading;
using Qplex.Messages;
using Qplex.Messages.Handlers;
// ReSharper disable InconsistentlySynchronizedField

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Thread which handles messages and invokes their handlers when they arrive.
    /// </summary>
    public class DispatcherThread
    {
        /// <summary>
        /// Thread
        /// </summary>
        private readonly Thread _thread;

        /// <summary>
        /// Message handlers dictionary
        /// </summary>
        private readonly IDictionary<Type, Communication.MessageHandler> _messageHandlers;

        /// <summary>
        /// Messages queue
        /// </summary>
        private readonly IMessagesIterator _messagesIterator;

        /// <summary>
        /// Indicates whether the thread should stop
        /// </summary>
        private bool _stop;

        /// <summary>
        /// Thread's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Thread's name</param>
        /// <param name="messagesIterator">Messages iterator</param>
        public DispatcherThread(string name, IMessagesIterator messagesIterator)
        {
            Name = name;
            _messagesIterator = messagesIterator;
            _thread = new Thread(HandleMessages);
            _messageHandlers = new Dictionary<Type, Communication.MessageHandler>();
            _stop = false;
        }

        /// <summary>
        /// Start handling incoming messages
        /// </summary>
        /// <returns>Initiatoin status</returns>
        public bool Start()
        {
            // TODO: Log
            _thread.Start();
            return true;
        }

        /// <summary>
        /// Stop thread
        /// </summary>
        public void Stop()
        {
            _stop = true;
            _thread.Join();
        }

        /// <summary>
        /// Get the messages' types the threads handles
        /// </summary>
        /// <returns>IEnumerable of type Message</returns>
        public IEnumerable<Type> GetHandledMessages()
        {
            return _messageHandlers.Keys;
        }

        /// <summary>
        /// Add message handler to dictionary
        /// </summary>
        /// <param name="messageType">Message begin handled</param>
        /// <param name="messageHandler">Handler for the message</param>
        public void AddHandler(Type messageType, Communication.MessageHandler messageHandler)
        {
            //TODO: Log
            _messageHandlers.Add(messageType, messageHandler);
        }

        /// <summary>
        /// Receive new message to handle
        /// </summary>
        /// <param name="message">Message to handle</param>
        public void Dispatch(Message message)
        {
            _messagesIterator.Add(message);
        }

        /// <summary>
        /// Handle incoming messages
        /// </summary>
        private void HandleMessages()
        {
            //TODO: Log
            while (!_stop)
            {
                lock (_messagesIterator.Inventory)
                {
                    while (!_messagesIterator.HasNext())
                    {
                        //Sleep until there is a message to handle
                        Monitor.Wait(_messagesIterator.Inventory);
                    }
                }

                var message = _messagesIterator.Next();
                // Invoke handler
                _messageHandlers[message.GetType()].Invoke(message);
            }
        }
    }
}
