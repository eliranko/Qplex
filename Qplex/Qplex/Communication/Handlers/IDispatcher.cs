using System;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Dispatches messages between threads
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Start threads
        /// </summary>
        /// <returns>Initiation status</returns>
        bool Start();

        /// <summary>
        /// Stop all threads
        /// </summary>
        void Stop();

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="messageHandler">Message handler action</param>
        /// <param name="threadName">Handling thread name</param>
        void AddHandler(Type messageType, Action<Message> messageHandler, string threadName = "");

        /// <summary>
        /// Dispatch new message to threads
        /// </summary>
        /// <param name="message">Message to handle</param>
        void Dispatch(Message message);

        /// <summary>
        /// Does an handler exsits for the given message type
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>True if an handler exists, false otherwise</returns>
        bool IsHandled(Type messageType);
    }
}