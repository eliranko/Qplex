using System.Linq;
using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Parsers;

namespace Qplex.Networking.Protocols
{
    /// <summary>
    /// Protocol wraps agent and handles low level network message, that the layer isn't
    /// interested about (such as keep-alive).
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Protocol<TIterator> : Communicator<TIterator>, IProtocol
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Network parser
        /// </summary>
        private IParser _parser;

        /// <summary>
        /// Protocol to agent channel
        /// </summary>
        private readonly InternalChannel _protocolToParserChannel;

        /// <summary>
        /// Ctor
        /// </summary>
        public Protocol()
        {
            _protocolToParserChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToParserChannel");
            SubscribeToChannel(_protocolToParserChannel);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public Protocol(IParser parser)
        {
            _parser = parser;
            _protocolToParserChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToParserChannel");
            SubscribeToChannel(_protocolToParserChannel);
            _parser.SubscribeToChannel(_protocolToParserChannel);
        }

        /// <summary>
        /// Set parser
        /// </summary>
        /// <param name="parser">Network parser</param>
        public void SetParser(IParser parser)
        {
            Log(LogLevel.Trace, $"Setting new parser: {parser}");
            _parser?.UnsubscribeFromChannel(_protocolToParserChannel);
            _parser = parser;
            _parser.SubscribeToChannel(_protocolToParserChannel);
        }

        /// <summary>
        /// Starting agent
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            if (_parser.Start() && base.Start())
            {
                Log(LogLevel.Trace, $"Protocol: {Name} started successfully");
                return true;
            }

            Log(LogLevel.Fatal, $"Protocol: {Name} failed to start");
            return false;
        }

        /// <summary>
        /// Stopping agent
        /// </summary>
        public override void Stop()
        {
            _parser.Stop();
            base.Stop();
            Log(LogLevel.Debug, $"Protcol: {Name} stopped successfully");
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            Log(LogLevel.Trace, $"Sending message: {message.Name}");
            _parser.Send(message);
        }

        #region Message Handlers

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        [MessageHandler]
        public void HandleNewIncomingMessage(NewIncomingMessage message)
        {
            Log(LogLevel.Trace, $"Tunneling {message}");
            TunnelMessage(message, BroadcastUpwards);
        }

        /// <summary>
        /// Handle connection socket error message
        /// </summary>
        [MessageHandler]
        public void HandleConnectionSocketErrorMessage(ParserConnectionErrorMessage message)
        {
            Log(LogLevel.Trace, $"Handling {message}");
            _parser.RetrieveConnection();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Broadcast message to 
        /// </summary>
        /// <param name="message"></param>
        protected void BroadcastUpwards(Message message)
        {
            foreach (var channel in GetInternalChannels().Where(channel => !channel.Equals(_protocolToParserChannel)))
                channel.Broadcast(message, TypeGuid);
        }

        #endregion
    }

    /// <summary>
    /// Protocol implemented with queue messgea iterator
    /// </summary>
    public class Protocol : Protocol<QueueMessagesIterator>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Protocol()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        public Protocol(IParser parser)
            : base(parser)
        {
        }
    }
}
