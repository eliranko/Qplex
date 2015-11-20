using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking;
using Qplex.Networking.Listeners;
using Qplex.Networking.Protocols;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service used for listening on incoming clients
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    /// <typeparam name="TProtocol">Protocol</typeparam>
    public class ListenerNetService<TIterator, TProtocol> : NetService<TIterator> 
        where TIterator : IMessagesIterator, new() where TProtocol : IProtocol, new()
    {
        /// <summary>
        /// Protocols list
        /// </summary>
        private readonly IList<TProtocol> _protocolsList;

        /// <summary>
        /// Listener
        /// </summary>
        private readonly IListener _listener;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="listener">Listener</param>
        public ListenerNetService(IListener listener)
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
            if (_listener.Start() && base.Start())
            {
                Log(LogLevel.Trace, "ListenerNetService started successfully");
                return true;
            }

            Log(LogLevel.Fatal, "ListenerNetService failed to start");
            return false;
        }

        /// <summary>
        /// Stop listening and close all existing connecitons
        /// </summary>
        public override void Stop()
        {
            _listener.Stop();
            foreach (var protocol in _protocolsList)
            {
                protocol.Stop();
            }
            base.Stop();
            Log(LogLevel.Trace, "ListenerNetService stopped successfully");
        }

        /// <summary>
        /// Send message
        /// </summary>
        public override void Send(Message message)
        {
            Log(LogLevel.Debug, $"Sending message:{message.GetType().Name}");
            foreach (var protocol in _protocolsList)
                protocol.Send(message);
        }

        /// <summary>
        /// Handle new connection
        /// </summary>
        [MessageHandler]
        public void HandleNewConnectionMessage(NewConnectionMessage message)
        {
            Log(LogLevel.Debug, "Handling new connection...");
            var protocol = new TProtocol();
            protocol.SetAgent(message.Agent);
            AddProtocol(protocol);
        }

        #region Helpers

        /// <summary>
        /// Add protocol to service
        /// </summary>
        /// <param name="protocol">Protocol</param>
        private void AddProtocol(TProtocol protocol)
        {
            protocol.SubscribeToChannel(ServiceToProtocolChannel);
            protocol.Start();
            _protocolsList.Add(protocol);
        }

        #endregion
    }

    /// <summary>
    /// Net service listener implemented with queue message iterator
    /// </summary>
    /// <typeparam name="TProtocol">Protocol</typeparam>
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public class ListenerNetService<TProtocol> : ListenerNetService<QueueMessagesIterator, TProtocol>
        where TProtocol : IProtocol, new()
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="listener">listener</param>
        public ListenerNetService(IListener listener) : base(listener)
        {
        }
    }
}
