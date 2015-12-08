using System.Linq;
using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Parser;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service warps listeners and protocols of a specific type.
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public abstract class NetService<TIterator> : Communicator<TIterator>, INetService
        where TIterator : IMessagesIterator, new()
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
        public abstract void Send(Message message);

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        /// <param name="message">Incoming message</param>
        [MessageHandler]
        public void HandleNewIncomingMessage(NewIncomingMessage message)
        {
            Log(LogLevel.Trace, $"Tunneling new incoming message: {message}");
            TunnelMessage(message.Message, BroadcastUpwards);
        }

        /// <summary>
        /// Broadcast to all but the protocols
        /// </summary>
        protected void BroadcastUpwards(Message message)
        {
            Log(LogLevel.Trace, $"Broadcasting {message} upwards");
            BroadcastTo(message, GetInternalChannels().Where(channel => !channel.Equals(ServiceToProtocolChannel)));
        }
    }

    /// <summary>
    /// Net service implemented with queue message iterator
    /// </summary>
    public abstract class NetService : NetService<QueueMessagesIterator>
    {
        
    }
}
