using System;
using System.Collections.Generic;
using NLog;
using Qplex.Communication.Channels;
using Qplex.Messages;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Broadcaster broadcasrs message, but cannot receive messages.
    /// </summary>
    public class Broadcaster : LogWrapper, IBroadcaster
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
            Logger = LogManager.GetLogger(GetType().Name);
            _channelsList = new List<InternalChannel>();
            BroadcasterGuid = GetType().GUID;
        }

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        public void SubscribeToChannel(InternalChannel internalChannel)
        {
            _channelsList.Add(internalChannel);
            internalChannel.Subscribe(this);
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        public void UnsubscribeFromChannel(InternalChannel internalChannel)
        {
            _channelsList.Remove(internalChannel); // TODO: Test if works
            internalChannel.Unsubscribe(this);
        }

        /// <summary>
        /// Broadcast message to channels
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(Message message)
        {
            foreach (var channel in _channelsList)
            {
                Log(LogLevel.Trace, $"Broadcast message:{message.GetType().Name} to channel:{channel.Name}");
                channel.Broadcast(message, BroadcasterGuid);
            }
        }
    }
}
