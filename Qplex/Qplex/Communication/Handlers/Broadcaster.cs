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
        private readonly HashSet<IInternalChannel> _channelsList;

        /// <summary>
        /// Ctor
        /// </summary>
        public Broadcaster()
        {
            _channelsList = new HashSet<IInternalChannel>();

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
        public bool SubscribeToChannel(IInternalChannel internalChannel)
        {
            if (internalChannel == null)
            {
                Log(LogLevel.Error, "Received empty internal channel to subscribe to");
                return false;
            }

            if (!_channelsList.Add(internalChannel))
            {
                Log(LogLevel.Error, $"Error adding channel: {internalChannel.Name} to broadcaster's list of channels");
                return false;
            }

            if (internalChannel.Subscribe(this)) return true;
            Log(LogLevel.Error, $"Error subscribing broadcaster to channel: {internalChannel.Name}");
            return false;
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="internalChannel">Channel</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        public bool UnsubscribeFromChannel(IInternalChannel internalChannel)
        {
            if (internalChannel == null)
            {
                Log(LogLevel.Error, "Received empty internal channel to unsubscribe from");
                return false;
            }

            if (!_channelsList.Remove(internalChannel))
            {
                Log(LogLevel.Error, $"Error removing channel: {internalChannel.Name} from broadcaster's list of channels");
                return false;
            }

            if (internalChannel.Unsubscribe(this)) return true;
            Log(LogLevel.Error, $"Error unsubscribing broadcaster from channel: {internalChannel.Name}");
            return false;
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

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            var broadcaster = obj as Broadcaster;
            if (broadcaster == null) return false;

            return Name == broadcaster.Name && TypeGuid == broadcaster.TypeGuid;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
    }
}
