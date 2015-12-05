using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Connection;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Connection;
using Qplex.Networking.FramingAlgorithms;
using Qplex.Networking.MessageFactories;

namespace Qplex.Networking.Parsers
{
    /// <summary>
    /// Parser sends and receives Message objects.
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Parser<TIterator> : Communicator<TIterator>, IParser
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Connectoin
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        /// Message factory
        /// </summary>
        private readonly IMessageFactory _messageFactory;

        /// <summary>
        /// Framing algorithm
        /// </summary>
        private readonly IFramingAlgorithm _framingAlgorithm;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="messageFactory">Message factory</param>
        /// <param name="framingAlgorithm">Framing algorithm</param>
        public Parser(IConnection connection, IMessageFactory messageFactory, IFramingAlgorithm framingAlgorithm)
        {
            _connection = connection;
            _connection.HeaderSize = framingAlgorithm.HeaderSize;
            _messageFactory = messageFactory;
            _framingAlgorithm = framingAlgorithm;

            var channel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToConnectionChannel");
            SubscribeToChannel(channel);
            _connection.SubscribeToChannel(channel);
        }

        /// <summary>
        /// Start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            if (_connection.ConnectAndReceive() == ConnectionConnectStatus.Success && base.Start())
            {
                Log(LogLevel.Trace, $"Parser: {Name} started successfully");
                return true;
            }

            Log(LogLevel.Fatal, $"Parser: {Name} failed to start");
            return false;
        }

        /// <summary>
        /// Stopping connection
        /// </summary>
        public override void Stop()
        {
            _connection.Close();
            base.Stop();
            Log(LogLevel.Trace, $"Parser: {Name} stopped successfully");
        }

        /// <summary>
        /// Send serialized message
        /// </summary>
        public void Send(Message message)
        {
            Log(LogLevel.Trace, $"Sending message: {message.Name}");
            _connection.Send(_framingAlgorithm.FrameBuffer(_messageFactory.Serialize(message)));
        }

        /// <summary>
        /// Retrieve connection
        /// </summary>
        public void RetrieveConnection()
        {
            Log(LogLevel.Trace, "Retrieving connection...");
            _connection.ConnectAndReceive();
        }

        /// <summary>
        /// Handle received buffer from connection
        /// </summary>
        [MessageHandler]
        public void HandleConnectionBufferReceivedMessage(ConnectionBufferReceivedMessage message)
        {
            Log(LogLevel.Trace, $"Handling {message}");
            //TODO: Handle socket failure
            Broadcast(
                new NewIncomingMessage(_messageFactory.Deserialize(_framingAlgorithm.UnframeBuffer(message.Buffer))));
        }

        /// <summary>
        /// Handle send status received from the connection
        /// </summary>
        [MessageHandler]
        public void HandleConnectionSendStatusMessage(ConnectionSendStatusMessage message)
        {
            Log(LogLevel.Trace, $"Handling {message}");

            if (message.ConnectionSocketStatus == ConnectionSocketStatus.Success) return;
            Log(LogLevel.Error, "Connection socket has failed, notifying agent...");
            Broadcast(new ParserConnectionErrorMessage());
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            return
                $"Parser with connection: {_connection.GetType().Name}, Message factory: {_messageFactory.GetType().Name} and Framing algorithm: {_framingAlgorithm.GetType().Name}";
        }
    }

    /// <summary>
    /// Parser implemented using queue message iterator
    /// </summary>
    public class Parser : Parser<QueueMessagesIterator>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="messageFactory">Message factory</param>
        /// <param name="framingAlgorithm">Framing algorithm</param>
        public Parser(IConnection connection, IMessageFactory messageFactory, IFramingAlgorithm framingAlgorithm) 
            : base(connection, messageFactory, framingAlgorithm)
        {
        }
    }
}
