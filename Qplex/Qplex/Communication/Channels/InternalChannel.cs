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
    public class InternalChannel : LogWrapper, IInternalChannel
    {
        /// <summary>
        /// Subscribers
        /// </summary>
        private readonly IList<IBroadcaster> _subscribersList;

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
            _subscribersList = new List<IBroadcaster>();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Channel's name</param>
        public InternalChannel(string name)
        {
            Name = name;
            Logger = LogManager.GetLogger(name);
            _subscribersList = new List<IBroadcaster>();
        }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        public bool Subscribe(Broadcaster broadcaster)
        {
            if (broadcaster == null)
            {
                Log(LogLevel.Error, "Tried to subscribe null broadcaster. Ignoring request.");
                return false;
            }

            Log(LogLevel.Trace, $"New subscriber {broadcaster.GetType().Name}");
            _subscribersList.Add(broadcaster);
            return true;
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        public bool Unsubscribe(Broadcaster broadcaster)
        {
            if (broadcaster == null)
            {
                Log(LogLevel.Error, "Tried to unsubscribe null broadcaster. Ignoring request.");
                return false;
            }

            var status = _subscribersList.Remove(broadcaster);
            if(status)
                Log(LogLevel.Trace, $"Unsubscriber {broadcaster.GetType().Name}");
            else
                Log(LogLevel.Warn, $"Failed unsubscribing {broadcaster.GetType().Name}");
            return _subscribersList.Remove(broadcaster);
        }

        /// <summary>
        /// Broadcast message to all but the caller
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller guid</param>
        public void Broadcast(Message message, Guid callerGuid)
        {
            foreach (var subscriber in _subscribersList.Where(subscriber =>
                subscriber.TypeGuid != callerGuid &&
                subscriber.GetType().GetInterfaces().Contains(typeof(ICommunicator))))
            {
                Log(LogLevel.Debug, $"Broadcasted {message.Name} to {subscriber.Name}");
                ((ICommunicator)subscriber).NewMessage(message);
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
