using Qplex.Communication.Handlers;

namespace Qplex.Networking
{
    /// <summary>
    /// Connection holds the socket
    /// </summary>
    public interface IConnection : IBroadcaster
    {
        /// <summary>
        /// Connect
        /// </summary>
        void Connect();

        /// <summary>
        /// Close connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        /// <param name="buffer">Buffer</param>
        void Send(byte[] buffer);
    }
}
