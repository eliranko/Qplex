using System.Collections.Generic;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.MessageFactories;
using Qplex.Messages;
using Qplex.Messages.Networking;

namespace Qplex.Networking
{
    /// <summary>
    /// Net service warps listeners and protocols of a specific type.
    /// </summary>
    /// <typeparam name="T">Protocol</typeparam>
    /// <typeparam name="TU">Listener</typeparam>
    public class NetService<T, TU> : Communicator where T : Protocol, new() where TU : Listener<IConnection, IMessageFactory>
    {
        /// <summary>
        /// Protocols list
        /// </summary>
        private readonly IList<T> _protocolsList;

        /// <summary>
        /// Listener
        /// </summary>
        private readonly TU _listener;

        /// <summary>
        /// Layer to protocols channel
        /// </summary>
        private readonly InternalChannel _serviceToProtocolsChannel;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="listener">Listener</param>
        public NetService(TU listener)
        {
            _listener = listener;
            _protocolsList = new List<T>();
            _serviceToProtocolsChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToProtocolsChannel");
            SubscribeToChannel(_serviceToProtocolsChannel);
            var serviceToListenerChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToListenerChannel");
            SubscribeToChannel(serviceToListenerChannel);
            _listener.SubscribeToChannel(serviceToListenerChannel);
        }

        /// <summary>
        /// Start listening
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            return base.Start() || _listener.Start();
        }

        /// <summary>
        /// Stop listening
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            _listener.Stop();
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            foreach (var protocol in _protocolsList)
            {
                protocol.Send(message);
            }
        }

        /// <summary>
        /// Handle new connection
        /// </summary>
        /// <param name="message">NewConnectionMessage</param>
        [MessageHandler]
        public void HandleNewConnectionMessage(NewConnectionMessage message)
        {
            var protocol = new T();
            protocol.SetAgent(message.Agent);
            _protocolsList.Add(protocol);
            protocol.SubscribeToChannel(_serviceToProtocolsChannel);
        }
    }
}
