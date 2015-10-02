using Qplex.Communication.Handlers;

namespace Qplex.Networking
{
    /// <summary>
    /// Connection holds the socket
    /// </summary>
    public interface IConnection : IBroadcaster
    {
        /// <summary>
        /// Send bytes over socket
        /// </summary>
        /// <param name="bytes">Bytes</param>
        void Send(byte[] bytes);
    }
}
