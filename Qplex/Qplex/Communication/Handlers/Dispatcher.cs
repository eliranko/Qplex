using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Dispatches messages between threads
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public class Dispatcher<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Threads list
        /// </summary>
        private readonly IList<DispatcherThread> _threadsList;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Dispatcher's name</param>
        public Dispatcher(string name)
        {
            _logger = LogManager.GetLogger(name);
            _threadsList = new List<DispatcherThread>();
        }

        /// <summary>
        /// Start threads
        /// </summary>
        /// <returns>Initiation status</returns>
        public bool Start()
        {
            var status = true;
            foreach (var dispatcherThread in _threadsList)
            {
                status = status && dispatcherThread.Start();
            }

            return status;
        }

        /// <summary>
        /// Stop all threads
        /// </summary>
        public void Stop()
        {
            _logger.Log(LogLevel.Debug, "Starting...");
            foreach (var dispatcherThread in _threadsList)
            {
                dispatcherThread.Stop();
            }
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageHandler">Message handler action</param>
        /// <param name="threadName">Handling thread name</param>
        public void AddHandler(Type messageType, Action<Message> messageHandler, string threadName = "")
        {
            _logger.Log(LogLevel.Debug, $"Adding handler of message:{messageType.Name}");
            var dispatcherThread = GetThread(threadName) ?? CreateThread(threadName);

            dispatcherThread.AddHandler(messageType, messageHandler);
        }

        /// <summary>
        /// Dispatch new message to threads
        /// </summary>
        /// <param name="message">Message to handle</param>
        public void Dispatch(Message message)
        {
            var dispatcherThread = GetThread(message.GetType());
            _logger.Log(LogLevel.Debug, $"Dispatcher message:{message.GetType().Name} to thread:{dispatcherThread?.Name}");

            //Handle message if there's an handler for it
            dispatcherThread?.Dispatch(message);
        }

        #region Helpers

        /// <summary>
        /// Get the thread which handles the corresponding message type
        /// </summary>
        /// <param name="messageType">Message type to handle</param>
        /// <returns>DisptacherThread if exists, null otherwise.</returns>
        private DispatcherThread GetThread(Type messageType)
        {
            return _threadsList.FirstOrDefault(thread => thread.GetHandledMessages().Contains(messageType));
        }

        /// <summary>
        /// Get thread by name
        /// </summary>
        /// <param name="name">Thread's name</param>
        /// <returns>DisptacherThread if exists, null otherwise.</returns>
        private DispatcherThread GetThread(string name)
        {
            return _threadsList.FirstOrDefault(thread => thread.Name == name);
        }

        /// <summary>
        /// Create DispatcherThread with the given name
        /// </summary>
        /// <param name="name">New thread's name</param>
        private DispatcherThread CreateThread(string name)
        {
            _logger.Log(LogLevel.Debug, $"Creating thread {name}");
            var thread = new DispatcherThread(name, new TIterator());
            _threadsList.Add(thread);

            return thread;
        }

        #endregion
    }
}
