using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking.Agent;
using Qplex.Networking.Agents;

namespace Qplex.Networking.Protocols
{
    /// <summary>
    /// Protocol wraps agent and handles low level network message, that the layer isn't
    /// interested about (such as keep-alive).
    /// </summary>
    public interface IProtocol : ICommunicator
    {
        /// <summary>
        /// Set agent
        /// </summary>
        /// <param name="agent">Network agent</param>
        void SetAgent(IAgent agent);

        /// <summary>
        /// Send message
        /// </summary>
        void Send(Message message);

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        [MessageHandler]
        void HandleNewIncomingMessage(NewIncomingMessage message);
    }
}