using System;
using System.Collections.Generic;
using System.Linq;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Dispatches messages between threads
    /// </summary>
    public class Dispatcher
    {
        /// <summary>
        /// Threads list
        /// </summary>
        private readonly IList<DispatcherThread> _threadsList;

        /// <summary>
        /// Messages iterator type to be used by this dispatcher's threads
        /// </summary>
        private readonly Type _messagesIteratorType;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagesIteratorType">Messages iterator type</param>
        public Dispatcher(Type messagesIteratorType)
        {
            _messagesIteratorType = messagesIteratorType;
            _threadsList = new List<DispatcherThread>();
        }

        /// <summary>
        /// Start threads
        /// </summary>
        /// <returns>Initiation status</returns>
        public bool Start()
        {
            //TODO: Log
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
            foreach (var dispatcherThread in _threadsList)
            {
                dispatcherThread.Stop();
            }
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageHandler">Message handler</param>
        /// <param name="threadName">Handling thread name</param>
        public void AddHandler(Type messageType, Delegate messageHandler, string threadName = "")
        {
            //TODO: Log
            var dispatcherThread = GetThread(threadName);

            //If the thread does not exist, create it
            if (dispatcherThread == null)
            {
                //TODO: Log
                CreateThread(threadName);
            }

// ReSharper disable once PossibleNullReferenceException
            dispatcherThread.AddHandler(messageType, messageHandler);
        }

        /// <summary>
        /// Dispatch new message to threads
        /// </summary>
        /// <param name="message">Message to handle</param>
        public void Dispatch(Message message)
        {
            //TODO: Log
            var dispatcherThread = GetThread(message.GetType());

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
        private void CreateThread(string name)
        {
            var messagesIteratorInstance = (IMessagesIterator) Activator.CreateInstance(_messagesIteratorType);
            _threadsList.Add(new DispatcherThread(name, messagesIteratorInstance));
        }

        #endregion
    }
}
