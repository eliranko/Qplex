using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking;

namespace Qplex.Networking
{
    /// <summary>
    /// Network agent. Agent sends and receives messsages over network.
    /// </summary>
    public class Agent : Communicator
    {
        /// <summary>
        /// Parser
        /// </summary>
        private readonly Parser _parser;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        public Agent(Parser parser)
        {
            _parser = parser;
            var channel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToParserChannel");
            SubscribeToChannel(channel);
            _parser.SubscribeToChannel(channel);
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
        /// Handle deserialized message
        /// </summary>
        /// <param name="message">DeserializedMessage</param>
        [MessageHandler]
        public void HandleDeserializedMessage(DeserializedMessage message)
        {
            //TODO: Log
            Broadcast(message.Message);
        }
    }
}
