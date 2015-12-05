using System;
using System.Collections.Generic;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using System.Linq;

namespace Qplex.Communication.Channels
{
    /// <summary>
    /// Internal channel that passes messages between subscribers, in publisher-subscriber manner.
    /// </summary>
    public interface IInternalChannel
    {
        /// <summary>
        /// Channel's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull subscription, false otherwise.</returns>
        bool Subscribe(IBroadcaster broadcaster);

        /// <summary>
        /// Unsubscribe from channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        /// <returns>True on successfull unsubscription, false otherwise.</returns>
        bool Unsubscribe(IBroadcaster broadcaster);

        /// <summary>
        /// Broadcast message to all but the publisher
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="callerGuid">Caller's guid</param>
        void Broadcast(Message message, Guid callerGuid);
    }

    /// <summary>
    /// Broadcasters container
    /// </summary>
    internal class BroadcasterContainer
    {
        private readonly HashSet<IBroadcaster> _broadcasters;
        private readonly HashSet<ICommunicator> _communicators;

        /// <summary>
        /// Ctor
        /// </summary>
        public BroadcasterContainer()
        {
            _broadcasters = new HashSet<IBroadcaster>();
            _communicators = new HashSet<ICommunicator>();
        }

        /// <summary>
        /// Add broadcaster
        /// </summary>
        /// <param name="broadcaster">Broadcaster</param>
        /// <returns>True on success, false otherwise</returns>
        public bool Add(IBroadcaster broadcaster)
        {
            var status = _broadcasters.Add(broadcaster);
            if (broadcaster.GetType().GetInterfaces().Contains(typeof (ICommunicator)))
                status |= _communicators.Add((ICommunicator)broadcaster);

            return status;
        }

        /// <summary>
        /// Remove broadcaster
        /// </summary>
        /// <param name="broadcaster">Broadcaster</param>
        /// <returns>True on success, false otherwise</returns>
        public bool Remove(IBroadcaster broadcaster)
        {
            var status = _broadcasters.Remove(broadcaster);
            if (broadcaster.GetType().GetInterfaces().Contains(typeof(ICommunicator)))
                status |= _communicators.Remove((ICommunicator)broadcaster);

            return status;
        }

        /// <summary>
        /// Get communicators expect the given ones
        /// </summary>
        /// <param name="excludedGuid">Excluded communicator (if it is one)</param>
        /// <returns>IEnumerable of communicators</returns>
        public IEnumerable<ICommunicator> GetCommunicators(Guid excludedGuid)
        {
            return _communicators.Where(communicator => communicator.TypeGuid != excludedGuid);
        }
    }
}