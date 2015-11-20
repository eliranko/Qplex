using Qplex.Communication.Handlers;

namespace Qplex.Networking.Connection
{
    /// <summary>
    /// IConnection connect stauts
    /// </summary>
    public enum ConnectionConnectStatus
    {
        /// <summary>
        /// Connected successfully
        /// </summary>
        Success,
        /// <summary>
        /// Given address is null
        /// </summary>
        NullAddress,
        /// <summary>
        /// Given portis invalid
        /// </summary>
        InvalidPort,
        /// <summary>
        /// Socket is already connceted
        /// </summary>
        SocketAlreadyConnected,
        /// <summary>
        /// An error occurred when accessing the socket. See the Remarks section for more information. 
        /// </summary>
        SocketError,
        /// <summary>
        /// Client is closed
        /// </summary>
        ClientDisposed
    }

    /// <summary>
    /// IConnection read/write status
    /// </summary>
    public enum ConnectionSocketStatus
    {
        /// <summary>
        /// Read/Wrote bytes successfully
        /// </summary>
        Success,
        /// <summary>
        /// Error which cannot be handled
        /// </summary>
        Error,
        /// <summary>
        /// The underlying Socket is closed
        /// </summary>
        SocketClosed,
        /// <summary>
        /// The NetworkStream is closed
        /// </summary>
        ClientDisposed
    }

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
        /// Connect and start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        ConnectionConnectStatus ConnectAndReceive();

        /// <summary>
        /// Closes connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send buffer over socket
        /// </summary>
        void Send(byte[] buffer);
    }
}
