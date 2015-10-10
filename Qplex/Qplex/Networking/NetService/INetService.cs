using Qplex.Communication.Handlers;
using Qplex.Messages;

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
    }
}