using Qplex.Communication.Handlers;
using Qplex.Messages;
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
        /// Connect
        /// </summary>
        void Connect();

        /// <summary>
        /// Close conneciton
        /// </summary>
        void Close();

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        void Send(Message message);
    }
}