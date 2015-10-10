using Qplex.Communication.Handlers;

namespace Qplex.Networking.Connection
{
    /// <summary>
    /// Connection holds the socket
    /// </summary>
    public interface IConnection : IBroadcaster
    {
        /// <summary>
        /// Header size
        /// </summary>
        int HeaderSize { get; set; }

        /// <summary>
        /// Connect
        /// </summary>
        void Connect();

        /// <summary>
        /// Close connection
        /// </summary>
        void Close();

        /// <summary>
        /// Start receiving message
        /// </summary>
        /// <returns>Operation status</returns>
        bool Start();

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        /// <param name="buffer">Buffer</param>
        void Send(byte[] buffer);
    }
}
