using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service used for listening on incoming clients
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    /// <typeparam name="TProtocol">Protocol</typeparam>
    public class ListenerNetService<TIterator, TProtocol> : NetService<TIterator> 
        where TIterator : IMessagesIterator, new() where TProtocol : Protocol, new()
    {
        /// <summary>
        /// Protocols list
        /// </summary>
        private readonly IList<TProtocol> _protocolsList;

        /// <summary>
        /// Listener
        /// </summary>
        private readonly Listener _listener;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="listener">Listener</param>
        public ListenerNetService(Listener listener)
        {
            _listener = listener;
            _protocolsList = new List<TProtocol>();
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
        /// Stop listening and close all existing connecitons
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            _listener.Stop();
            foreach (var protocol in _protocolsList)
            {
                protocol.Close();
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public override void Send(Message message)
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
            var protocol = new TProtocol();
            protocol.SetAgent(message.Agent);
            _protocolsList.Add(protocol);
            protocol.SubscribeToChannel(ServiceToProtocolChannel);
        }
    }

    /// <summary>
    /// Net service listener implemented with queue message iterator
    /// </summary>
    /// <typeparam name="TProtocol">Protocol</typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public class ListenerNetService<TProtocol> : ListenerNetService<QueueMessagesIterator, TProtocol>
        where TProtocol : Protocol, new()
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="listener">listener</param>
        public ListenerNetService(Listener listener) : base(listener)
        {
        }
    }
}
