using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking.Parser;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service warps listeners and protocols of a specific type.
    /// </summary>
    public interface INetService : ICommunicator
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        void Send(Message message);

        /// <summary>
        /// Handle new incoming message
        /// </summary>
        /// <param name="message">Incoming message</param>
        [MessageHandler]
        void HandleNewIncomingMessage(NewIncomingMessage message);
    }
}