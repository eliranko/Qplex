using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking;
using Qplex.Networking.Parsers;

namespace Qplex.Networking.Agents
{
    /// <summary>
    /// Network agent. Agent sends and receives messsages over network.
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Agent<TIterator> : Communicator<TIterator>, IAgent where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Parser
        /// </summary>
        private readonly IParser _parser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        public Agent(IParser parser)
        {
            _parser = parser;
            var channel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToParserChannel");
            SubscribeToChannel(channel);
            _parser.SubscribeToChannel(channel);
        }

        /// <summary>
        /// Start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            return _parser.Start() && base.Start();
        }

        /// <summary>
        /// Connect
        /// </summary>
        public void Connect()
        {
            _parser.Connect();
        }

        /// <summary>
        /// Close conneciton
        /// </summary>
        public void Close()
        {
            _parser.Close();
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            _parser.Send(message);
        }

        /// <summary>
        /// Handle unframed message
        /// </summary>
        /// <param name="message">UnframedBufferMessage</param>
        [MessageHandler]
        public void HandleUnframedBufferMessage(UnframedBufferMessage message)
        {
            //TODO: Log
            Broadcast(message.Message);
        }
    }

    /// <summary>
    /// Agent implemented with queue message iterator
    /// </summary>
    public class Agent : Agent<QueueMessagesIterator>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        public Agent(IParser parser) : base(parser)
        {
        }
    }
}
