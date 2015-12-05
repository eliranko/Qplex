using System;
using System.Collections.Generic;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Thread which handles messages and invokes their handlers when they arrive.
    /// </summary>
    public interface IDispatcherThread
    {
        /// <summary>
        /// Thread's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Start handling incoming messages
        /// </summary>
        /// <returns>Initiatoin status</returns>
        bool Start();

        /// <summary>
        /// Stop thread
        /// </summary>
        void Stop();

        /// <summary>
        /// Get the messages' types the threads handles
        /// </summary>
        /// <returns>IEnumerable of type Message</returns>
        IEnumerable<Type> GetHandledMessages();

        /// <summary>
        /// Add message handler to dictionary
        /// </summary>
        /// <param name="messageType">Message begin handled</param>
        /// <param name="messageHandler">Action handler for the message</param>
        void AddHandler(Type messageType, Action<Message> messageHandler);

        /// <summary>
        /// Receive new message to handle
        /// </summary>
        /// <param name="message">Message to handle</param>
        void Dispatch(Message message);
        
    }
}