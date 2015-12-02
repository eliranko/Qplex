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
    public class Dispatcher<TIterator> : LogWrapper
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Threads list
        /// </summary>
        private readonly List<DispatcherThread> _threadsList;

        /// <summary>
        /// Dispatcher's name
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Dispatcher's name</param>
        public Dispatcher(string name)
        {
            _name = name;
            Logger = LogManager.GetLogger(name);
            _threadsList = new List<DispatcherThread>();
        }

        /// <summary>
        /// Start threads
        /// </summary>
        /// <returns>Initiation status</returns>
        public bool Start()
        {
            Log(LogLevel.Trace, $"Starting dispatcher: {_name}");
            return _threadsList.Aggregate(true, (current, dispatcherThread) => current && dispatcherThread.Start());
        }

        /// <summary>
        /// Stop all threads
        /// </summary>
        public void Stop()
        {
            Log(LogLevel.Debug, $"Stopping dispatcher: {_name}");
            foreach (var dispatcherThread in _threadsList)
                dispatcherThread.Stop();
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageHandler">Message handler action</param>
        /// <param name="threadName">Handling thread name</param>
        public void AddHandler(Type messageType, Action<Message> messageHandler, string threadName = "")
        {
            Log(LogLevel.Trace, $"Adding handler of message:{messageType.Name}");

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
            Log(LogLevel.Trace,
                $"Dispatcher message:{message.Name} to thread:{dispatcherThread?.Name ?? $"Message is not being handled by currect dispatcher: {_name}"}");

            //Handle message if there's an handler for it
            dispatcherThread?.Dispatch(message);
        }

        /// <summary>
        /// Does an handler exsits for the given message type
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>True if an handler exists, false otherwise</returns>
        public bool IsHandled(Type messageType)
        {
            return _threadsList.Exists(thread => thread.GetHandledMessages().Contains(messageType));
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
            Log(LogLevel.Debug, $"Creating thread {name}");
            var thread = new DispatcherThread(name, new TIterator());
            _threadsList.Add(thread);

            return thread;
        }

        #endregion
    }
}
