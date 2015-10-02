using System;
using System.Collections.Generic;
using Qplex.Communication.Channels;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Broadcaster broadcasrs message, but cannot receive messages.
    /// </summary>
    public class Broadcaster : IBroadcaster
    {
        /// <summary>
        /// Type guid
        /// </summary>
        public Guid BroadcasterGuid { get; }

        /// <summary>
        /// Channels list
        /// </summary>
        private readonly IList<InternalChannel> _channelsList;

        /// <summary>
        /// Ctor
        /// </summary>
        public Broadcaster()
        {
            _channelsList = new List<InternalChannel>();
            BroadcasterGuid = GetType().GUID;
        }

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        public void SubscribeToChannel(InternalChannel internalChannel)
        {
            //TODO: Log
            _channelsList.Add(internalChannel);
            internalChannel.Subscribe(this);
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        public void UnsubscribeFromChannel(InternalChannel internalChannel)
        {
            //TODO: Log
            _channelsList.Remove(internalChannel); // TODO: Test if works
            internalChannel.Unsubscribe(this);
        }

        /// <summary>
        /// Broadcast message to channels
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(Message message)
        {
            //TODO: Log
            foreach (var channel in _channelsList)
            {
                channel.Broadcast(message, BroadcasterGuid);
            }
        }
    }
}
