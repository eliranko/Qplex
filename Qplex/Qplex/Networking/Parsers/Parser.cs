using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.FramingAlgorithms;
using Qplex.MessageFactories;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking;
using Qplex.Networking.Connection;

namespace Qplex.Networking.Parsers
{
    /// <summary>
    /// Parser sends and receives Message objects.
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Parser<TIterator> : Communicator<TIterator>, IParser where TIterator : IMessagesIterator, new()
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
            return _connection.Start() && base.Start();
        }

        /// <summary>
        /// Connect
        /// </summary>
        public void Connect()
        {
            _connection.Connect();
        }

        /// <summary>
        /// Close conneciton
        /// </summary>
        public void Close()
        {
            _connection.Close();
        }

        /// <summary>
        /// Send serialized message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            _connection.Send(_framingAlgorithm.FrameBuffer(_messageFactory.Serialize(message)));
        }

        /// <summary>
        /// Handle received buffer
        /// </summary>
        /// <param name="message">NewBufferReceivedMessage</param>
        [MessageHandler]
        public void HandleNewBufferReceivedMessage(BufferReceivedMessage message)
        {
            //TODO: Log
            Broadcast(new UnframedBufferMessage(
                _messageFactory.Deserialize(_framingAlgorithm.UnframeBuffer(message.Buffer))));
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
