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
        public Guid TypeGuid { get; }

        /// <summary>
        /// Broadcaster's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Channels list
        /// </summary>
        private readonly IList<IInternalChannel> _channelsList;

        /// <summary>
        /// Ctor
        /// </summary>
        public Broadcaster()
        {
            _channelsList = new List<IInternalChannel>();

            var myType = GetType();
            Name = myType.Name;
            Logger = LogManager.GetLogger(myType.Name);
            TypeGuid = myType.GUID;
        }

        /// <summary>
        /// Get the subscribed internal channels
        /// </summary>
        public IEnumerable<IInternalChannel> GetInternalChannels()
        {
            return _channelsList;
        }

        /// <summary>
        /// Subscribe to channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        public bool SubscribeToChannel(InternalChannel internalChannel)
        {
            _channelsList.Add(internalChannel);
            return internalChannel.Subscribe(this);
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        public bool UnsubscribeFromChannel(InternalChannel internalChannel)
        {
            if (!_channelsList.Remove(internalChannel))
                Log(LogLevel.Warn, $"Error removing channel {internalChannel.Name} from channel's list");

            return internalChannel.Unsubscribe(this);
        }

        /// <summary>
        /// Broadcast message to channels
        /// </summary>
        /// <param name="message">Message to broadcast</param>
        public void Broadcast(Message message)
        {
            foreach (var channel in _channelsList)
            {
                Log(LogLevel.Trace, $"Broadcast message:{message.Name} to channel:{channel.Name}");
                channel.Broadcast(message, TypeGuid);
            }
        }
    }
}
