using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Communication.Channels
{
    /// <summary>
    /// Channel is a tune which passes messages to all of its subscribers (publis/subscribe).
    /// </summary>
    public sealed class InternalChannel : LogWrapper, IInternalChannel
    {
        /// <summary>
        /// Subscribers
        /// </summary>
        private readonly BroadcasterContainer _subscribersList;

        /// <summary>
        /// Channel's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public InternalChannel()
        {
            Name = GetType().Name;
            Logger = LogManager.GetLogger(Name);
            _subscribersList = new BroadcasterContainer();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Channel's name</param>
        public InternalChannel(string name)
        {
            Name = name;
            Logger = LogManager.GetLogger(name);
            _subscribersList = new BroadcasterContainer();
        }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        public bool Subscribe(IBroadcaster broadcaster)
        {
            if (broadcaster == null)
            {
                Log(LogLevel.Error, "Tried to subscribe null broadcaster. Ignoring request.");
                return false;
            }

            var status = _subscribersList.Add(broadcaster);
            if (status)
                Log(LogLevel.Trace, $"New subscriber: {broadcaster.Name} from channel");
            else
                Log(LogLevel.Warn, $"Failed Subscribing {broadcaster.Name} from channel");
            return status;
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        public bool Unsubscribe(IBroadcaster broadcaster)
        {
            if (broadcaster == null)
            {
                Log(LogLevel.Error, "Tried to unsubscribe null broadcaster. Ignoring request.");
                return false;
            }

            var status = _subscribersList.Remove(broadcaster);
            if(status)
                Log(LogLevel.Trace, $"Unsubscribed {broadcaster.Name} from channel");
            else
                Log(LogLevel.Warn, $"Failed unsubscribing {broadcaster.Name} from channel");
            return status;
        }

        /// <summary>
        /// Broadcast message to all but the caller
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller guid</param>
        public void Broadcast(Message message, Guid callerGuid)
        {
            if (message == null)
            {
                Log(LogLevel.Error, "Received null message to broadcast");
                return;
            }

            foreach (var subscriber in _subscribersList.GetCommunicators(callerGuid))
            {
                Log(LogLevel.Debug, $"Broadcasted {message.Name} to {subscriber.Name}");
                subscriber.NewMessage(message);
            }
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var internalChannel = obj as InternalChannel;
            if (internalChannel == null) return false;

            return GetType().GUID == internalChannel.GetType().GUID;
        }

        /// <summary>
        /// Get Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
    }
}
