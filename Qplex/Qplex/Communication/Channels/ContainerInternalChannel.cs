using System;
using NLog;
using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Communication.Channels
{
    /// <summary>
    /// This channel as an owner (the container) which receives all of the message
    /// broadcasted from the other subscribers, and they receive all of the message that 
    /// the owner broadcasts.
    /// The subscribers cannot pass messages between themself.
    /// </summary>
    public class ContainerInternalChannel : LogWrapper, IInternalChannel
    {
        /// <summary>
        /// Container
        /// </summary>
        private readonly IBroadcaster _container;

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
        /// <param name="container">Container</param>
        public ContainerInternalChannel(IBroadcaster container)
        {
            _container = container;
            Name = GetType().Name;
            Logger = LogManager.GetLogger(Name);
            _subscribersList = new BroadcasterContainer();
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="name">Channel's name</param>
        public ContainerInternalChannel(IBroadcaster container, string name)
        {
            _container = container;
            Name = name;
            Logger = LogManager.GetLogger(Name);
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

            if (broadcaster.Equals(_container))
            {
                Log(LogLevel.Warn, "Tried to subscribe the container. ignoring request");
                return false;
            }

            var status = _subscribersList.Add(broadcaster);
            if (status)
                Log(LogLevel.Trace, $"New subscriber: {broadcaster.Name} to channel");
            else
                Log(LogLevel.Warn, $"Failed Subscribing {broadcaster.Name} to channel");
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

            if (broadcaster.Equals(_container))
            {
                Log(LogLevel.Warn, "Tried to unsubscribe the container. ignoring request");
                return false;
            }

            var status = _subscribersList.Remove(broadcaster);
            if (status)
                Log(LogLevel.Trace, $"Unsubscribed {broadcaster.Name} from channel");
            else
                Log(LogLevel.Warn, $"Failed unsubscribing {broadcaster.Name} from channel");
            return status;
        }

        /// <summary>
        /// Broadcast message to all but the publisher
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller's guid</param>
        public void Broadcast(Message message, Guid callerGuid)
        {
            if (message == null)
            {
                Log(LogLevel.Error, "Received null message to broadcast");
                return;
            }

            if (_container.TypeGuid != callerGuid)
            {
                Log(LogLevel.Trace, $"Broadcasted {message.Name} to the container");
                _container.Broadcast(message);
                return;
            }

            foreach (var subscriber in _subscribersList.GetCommunicators(callerGuid))
            {
                Log(LogLevel.Trace, $"Broadcasted {message.Name} to {subscriber.Name}");
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
            var internalChannel = obj as ContainerInternalChannel;
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

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ContainerInternalChannel {Name}";
        }
    }
}
