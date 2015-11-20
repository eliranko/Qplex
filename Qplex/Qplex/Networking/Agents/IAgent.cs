using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking.Parser;

namespace Qplex.Networking.Agents
{
    /// <summary>
    /// Network agent. Agent sends and receives messsages over network.
    /// </summary>
    public interface IAgent : ICommunicator
    {
        /// <summary>
        /// Send message
        /// </summary>
        void Send(Message message);

        /// <summary>
        /// Handle connection socket error message
        /// </summary>
        [MessageHandler]
        void HandleConnectionSocketErrorMessage(ParserConnectionErrorMessage message);

        /// <summary>
        /// Handle unframed message
        /// </summary>
        [MessageHandler]
        void HandleUnframedBufferMessage(ParserUnframedBufferMessage message);
    }
}