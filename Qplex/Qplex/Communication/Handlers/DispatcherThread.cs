using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
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
        /// Logger
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Thread
        /// </summary>
        private readonly Thread _thread;

        /// <summary>
        /// Message handlers dictionary
        /// </summary>
        private readonly IDictionary<Type, Action<Message>> _messageHandlers;

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
            _logger = LogManager.GetLogger(name);
            Name = name;
            _messagesIterator = messagesIterator;
            _thread = new Thread(HandleMessages);
            _messageHandlers = new Dictionary<Type, Action<Message>>();
            _stop = false;
        }

        /// <summary>
        /// Start handling incoming messages
        /// </summary>
        /// <returns>Initiatoin status</returns>
        public bool Start()
        {
            _logger.Log(LogLevel.Debug, "Starting...");
            _thread.Start();
            return true;
        }

        /// <summary>
        /// Stop thread
        /// </summary>
        public void Stop()
        {
            _logger.Log(LogLevel.Debug, "Stopping...");
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
        /// <param name="messageHandler">Action handler for the message</param>
        public void AddHandler(Type messageType, Action<Message> messageHandler)
        {
            _logger.Log(LogLevel.Debug, $"Adding handler of message:{messageType.Name}");
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
                        _logger.Log(LogLevel.Debug, "No more messages to handle. Going to sleep...");
                        Monitor.Wait(_messagesIterator.Inventory);
                    }
                }

                var message = _messagesIterator.Next();
                _logger.Log(LogLevel.Debug, $"Handling message:{message.GetType().Name}");
                //TODO: Maybe invoke using Task
                _messageHandlers[message.GetType()].Invoke(message);
            }
        }
    }
}
