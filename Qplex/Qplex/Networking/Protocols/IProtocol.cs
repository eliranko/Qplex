using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Parsers;

namespace Qplex.Networking.Protocols
{
    /// <summary>
    /// Protocol wraps agent and handles low level network message, that the layer isn't
    /// interested about (such as keep-alive).
    /// </summary>
    public interface IProtocol : ICommunicator
    {
        /// <summary>
        /// Set parser
        /// </summary>
        /// <param name="parser">Network parser</param>
        void SetParser(IParser parser);

        /// <summary>
        /// Send message
        /// </summary>
        void Send(Message message);

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        [MessageHandler]
        void HandleNewIncomingMessage(NewIncomingMessage message);

        /// <summary>
        /// Handle connection socket error message
        /// </summary>
        [MessageHandler]
        void HandleConnectionSocketErrorMessage(ParserConnectionErrorMessage message);
    }
}