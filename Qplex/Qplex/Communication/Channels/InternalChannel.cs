using System.Collections.Generic;
using System.Linq;
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
        /// Subscribers
        /// </summary>
        private readonly IList<Broadcaster> _subscribersList;

        /// <summary>
        /// Channel's name
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// Ctor
        /// </summary>
        public InternalChannel(string name)
        {
            _name = name;
            _subscribersList = new List<Broadcaster>();
        }

        /// <summary>
        /// Subscribe to the channel
        /// </summary>
        /// <param name="broadcaster">Subscriber</param>
        public void Subscribe(Broadcaster broadcaster)
        {
            if (broadcaster == null)
            {
                //TODO: Log
                return;
            }

            //TODO: Log
            _subscribersList.Add(broadcaster);
        }

        /// <summary>
        /// Broadcast message
        /// </summary>
        /// <param name="message"></param>
        public void Broadcast(Message message)
        {
            foreach (var subscriber in _subscribersList.Where(subscriber => subscriber.GetType() == typeof (Communicator)))
            {
                //TODO: Log
                ((Communicator) subscriber).NewMessage(message);
            }
        }
    }
}
