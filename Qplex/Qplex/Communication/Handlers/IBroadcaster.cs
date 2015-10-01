using System;
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
        Guid BroadcasterGuid { get; }

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        void SubscribeToChannel(InternalChannel internalChannel);

        /// <summary>
        /// Broadcast message to channels
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        void Broadcast(Message message);
    }
}
