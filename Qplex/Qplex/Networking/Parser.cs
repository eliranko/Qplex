using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.MessageFactories;
using Qplex.Messages;
using Qplex.Messages.Networking;

namespace Qplex.Networking
{
    public class Parser : Communicator
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
        /// Ctor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="messageFactory">Message factory</param>
        public Parser(IConnection connection, IMessageFactory messageFactory)
        {
            _connection = connection;
            _messageFactory = messageFactory;
            var channel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToConnectionChannel");
            SubscribeToChannel(channel);
            _connection.SubscribeToChannel(channel);
        }

        /// <summary>
        /// Send serialized message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            _connection.Send(_messageFactory.Serialize(message));
        }

        /// <summary>
        /// Handle new message received
        /// </summary>
        /// <param name="message">SerializedMessageReceivedMessage</param>
        [MessageHandler]
        public void HandleNewMessageReceivedMessage(SerializedMessageReceivedMessage message)
        {
            //TODO: Log
            Broadcast(new DeserializedMessage(_messageFactory.Deserialize(message.MessageArray)));
        }
    }
}
