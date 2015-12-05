using System;
using System.Collections.Generic;
using Qplex.Communication.Channels;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Broadcaster broadcasrs message, but cannot receive messages.
    /// </summary>
    public interface IBroadcaster
    {
        /// <summary>
        /// Type guid
        /// </summary>
        Guid TypeGuid { get; }

        /// <summary>
        /// Broadcaster's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the subscribed internal channels
        /// </summary>
        IEnumerable<IInternalChannel> GetInternalChannels();

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        bool SubscribeToChannel(IInternalChannel internalChannel);

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        /// <returns>True on successfull usubscription, false otherwise.</returns>
        bool UnsubscribeFromChannel(IInternalChannel internalChannel);

        /// <summary>
        /// Broadcast message to channels
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        void Broadcast(Message message);
    }
}
