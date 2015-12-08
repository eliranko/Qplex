using System;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Communicator broadcasts and receives messages.
    /// </summary>
    public interface ICommunicator : IBroadcaster
    {
        /// <summary>
        /// Load message handlers using reflection, and start dispatcher threads
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Stop dispatcher threads
        /// </summary>
        void Stop();

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New received message</param>
        void NewMessage(Message message);

        /// <summary>
        /// Notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        void Notify(Message message);

        /// <summary>
        /// Delayed notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="milliseconds">Delay time</param>
        void DelayedNotify(Message message, int milliseconds);

        /// <summary>
        /// Set initial number of message to receiving before starting layer
        /// </summary>
        /// <param name="messages">Messages requeired to initiate the communicator</param>
        void SetInitMessages(params Type[] messages);
        
        /// <summary>
        /// Handle message if an handler exists, otherwise invoke action with message
        /// </summary>
        void TunnelMessage(Message message, Action<Message> action);

        /// <summary>
        /// Expect a message
        /// </summary>
        /// <typeparam name="TMessage">Type of message expected</typeparam>
        /// <param name="timeout">The timeout until the action is invoked</param>
        /// <param name="errorMessage">The error message to action is invoked with</param>
        /// <param name="timeoutAction">The action to invoke incase of failure</param>
        void Expect<TMessage>(int timeout, Message errorMessage, Action<Message> timeoutAction) where TMessage : Message;
    }
}
