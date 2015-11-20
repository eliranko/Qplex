using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Agent;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Parsers;

namespace Qplex.Networking.Agents
{
    /// <summary>
    /// Network agent. Agent sends and receives messsages over network.
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Agent<TIterator> : Communicator<TIterator>, IAgent
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Parser
        /// </summary>
        private readonly IParser _parser;

        /// <summary>
        /// Ctor
        /// </summary>
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
            if (_parser.Start() && base.Start())
            {
                Log(LogLevel.Debug, "Agent started successfully");
                return true;
            }

            Log(LogLevel.Fatal, "Agent failed to start");
            return false;
        }

        /// <summary>
        /// Stopping parser
        /// </summary>
        public override void Stop()
        {
            _parser.Stop();
            base.Stop();
            Log(LogLevel.Debug, "Agent stopped successfully");
        }

        /// <summary>
        /// Send message
        /// </summary>
        public void Send(Message message)
        {
            Log(LogLevel.Trace, "Sending message through parser");
            _parser.Send(message);
        }

        /// <summary>
        /// Handle connection socket error message
        /// </summary>
        [MessageHandler]
        public void HandleConnectionSocketErrorMessage(ParserConnectionErrorMessage message)
        {
            Log(LogLevel.Trace, "Handling connection error");
            _parser.RetrieveConnection();
        }

        /// <summary>
        /// Handle unframed message
        /// </summary>
        [MessageHandler]
        public void HandleUnframedBufferMessage(ParserUnframedBufferMessage message)
        {
            Log(LogLevel.Trace, $"Handling new unframed buffer:{message.Message.GetType().Name}");
            Broadcast(new NewIncomingMessage(message.Message));
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
