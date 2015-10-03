using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service warps listeners and protocols of a specific type.
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public abstract class NetService<TIterator> : Communicator<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Layer to protocol channel
        /// </summary>
        protected readonly InternalChannel ServiceToProtocolChannel;

        /// <summary>
        /// Ctor
        /// </summary>
        protected NetService()
        {
            ServiceToProtocolChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToProtocolsChannel");
            SubscribeToChannel(ServiceToProtocolChannel);
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public abstract void Send(Message message);
    }

    /// <summary>
    /// Net service implemented with queue message iterator
    /// </summary>
    public abstract class NetService : Communicator<QueueMessagesIterator>
    {
        
    }
}
