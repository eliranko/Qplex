using System.Collections.Generic;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Dispatches messages between threads
    /// </summary>
    public class Dispatcher
    {
        /// <summary>
        /// Message handlers dictionary
        /// </summary>
        private readonly IDictionary<Message, Communication.MessageHandler> _messageHandlersDictionary;

        /// <summary>
        /// Ctor
        /// </summary>
        public Dispatcher()
        {
            _messageHandlersDictionary = new Dictionary<Message, Communication.MessageHandler>();
        }

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="handler">Message handler</param>
        /// <param name="message">Message</param>
        /// <param name="threadName">Handling thread name</param>
        public void AddHandler(Communication.MessageHandler handler, Message message, string threadName = "")
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        /// <param name="message">Message to handle</param>
        public void HandleMessage(Message message)
        {
            throw new System.NotImplementedException();
        }
    }
}
