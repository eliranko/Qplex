using System;
using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Communication.Channels
{
    /// <summary>
    /// Internal channel that passes messages between subscribers, in publisher-subscriber manner.
    /// </summary>
    public interface IInternalChannel
    {
        /// <summary>
        /// Channel's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        bool Subscribe(Broadcaster broadcaster);

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        bool Unsubscribe(Broadcaster broadcaster);

        /// <summary>
        /// Broadcast message to all but the publisher
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller's guid</param>
        void Broadcast(Message message, Guid callerGuid);
    }
}