using Qplex.Networking.Connection;

namespace Qplex.Messages.Networking.Connection
{
    /// <summary>
    /// New buffer received over socket message
    /// </summary>
    public class ConnectionBufferReceivedMessage : Message
    {
        /// <summary>
        /// Socket status
        /// </summary>
        public ConnectionSocketStatus ConnectionSocketStatus { get; }

        /// <summary>
        /// Buffer
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionSocketStatus">Connection socket status</param>
        /// <param name="buffer">Buffer received</param>
        public ConnectionBufferReceivedMessage(ConnectionSocketStatus connectionSocketStatus, byte[] buffer)
        {
            ConnectionSocketStatus = connectionSocketStatus;
            Buffer = buffer;
        }
    }
}
