using Qplex.Attributes;
using Qplex.Messages.Networking;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Net service used for listening on incoming clients
    /// </summary>
    public interface IListenerNetService : INetService
    {
        /// <summary>
        /// Handle new incoming connection
        /// </summary>
        /// <param name="message"></param>
        [MessageHandler]
        void HandleNewConnectionMessage(NewConnectionMessage message);
    }
}