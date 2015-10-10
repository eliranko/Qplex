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
    public class InternalChannel
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly Logger _logger;

        /// <summary>
        /// Channel's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Subscribers
        /// </summary>
        private readonly IList<Broadcaster> _subscribersList;

        /// <summary>
        /// Ctor
        /// </summary>
        public InternalChannel(string name)
        {
            Name = name;
            _logger = LogManager.GetLogger(name);
            _subscribersList = new List<Broadcaster>();
        }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        public void Subscribe(Broadcaster broadcaster)
        {
            _logger.Log(LogLevel.Debug, $"New subscriber {broadcaster.GetType().Name}");
            _subscribersList.Add(broadcaster);
        }

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        public void Unsubscribe(Broadcaster broadcaster)
        {
            _logger.Log(LogLevel.Debug, $"Unsubscriber {broadcaster.GetType().Name}");
            _subscribersList.Remove(broadcaster);
        }

        /// <summary>
        /// Broadcast message to all but the caller
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller guid</param>
        public void Broadcast(Message message, Guid callerGuid)
        {
            foreach (var subscriber in _subscribersList.Where(subscriber =>
                subscriber.BroadcasterGuid != callerGuid &&
                subscriber.GetType().GetInterfaces().Contains(typeof(ICommunicator))))
            {
                _logger.Log(LogLevel.Debug, $"Broadcasted {message.GetType().Name} to {subscriber.GetType().Name}");
                ((ICommunicator)subscriber).NewMessage(message);
            }
        }
    }
}
